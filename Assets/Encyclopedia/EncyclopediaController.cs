using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Keep the controller active while its Window canvas is hidden.
public class EncyclopediaController : MonoBehaviour
{
    [SerializeField] EncyclopediaEntry rootEntry;
    [SerializeField] EscapeMenuController pauseMenu;
    [SerializeField] GameObject window;
    [SerializeField] RectTransform treeContent;
    [SerializeField] EncyclopediaRow rowTemplate;
    [SerializeField] TMP_Text breadcrumb;
    [SerializeField] TMP_Text heading;
    [SerializeField] TMP_Text article;
    [SerializeField] RectTransform articleContent;
    [SerializeField] ScrollRect treeScroll;
    [SerializeField] ScrollRect articleScroll;
    [SerializeField] Button closeButton;

    readonly HashSet<EncyclopediaEntry> expanded = new HashSet<EncyclopediaEntry>();
    readonly List<EncyclopediaRow> rows = new List<EncyclopediaRow>();
    EncyclopediaIndex index;
    EncyclopediaEntry selected;
    CursorLockMode previousLock;
    bool previousVisible;
    float treeWidth;

    public bool IsOpen => window.activeSelf;

    void Awake()
    {
        window.SetActive(false);
        index = new EncyclopediaIndex(rootEntry);
        foreach (EncyclopediaEntry entry in index.Entries)
        {
            foreach (TMP_FontAsset font in entry.Fonts)
            {
                MaterialReferenceManager.AddFontAsset(font);
            }
        }
        selected = rootEntry;
        expanded.Add(rootEntry);
        closeButton.onClick.AddListener(Close);
    }

    void Update()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame && !pauseMenu.IsOpen)
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    void LateUpdate()
    {
        if (IsOpen && !Mathf.Approximately(treeWidth, treeContent.rect.width))
        {
            RebuildTree();
        }
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }
        previousLock = Cursor.lockState;
        previousVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        window.SetActive(true);
        Canvas.ForceUpdateCanvases();
        OpenArticle(selected.Id);
    }

    public void Close()
    {
        if (!IsOpen)
        {
            return;
        }
        window.SetActive(false);
        Cursor.lockState = previousLock;
        Cursor.visible = previousVisible;
    }

    public void OpenArticle(string id)
    {
        selected = index.Find(id);
        foreach (EncyclopediaEntry ancestor in index.PathTo(selected))
        {
            if (ancestor != selected)
            {
                expanded.Add(ancestor);
            }
        }
        ShowArticle();
        RebuildTree();
    }

    void ShowArticle()
    {
        breadcrumb.text = index.Breadcrumb(selected);
        heading.text = EncyclopediaIndex.Plain(selected.Title);
        article.text = index.ArticleText(selected);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(articleContent);
        articleScroll.StopMovement();
        articleScroll.verticalNormalizedPosition = 1f;
    }

    void RebuildTree()
    {
        treeWidth = treeContent.rect.width;
        float scroll = treeScroll.verticalNormalizedPosition;
        foreach (EncyclopediaRow row in rows)
        {
            row.gameObject.SetActive(false);
            Destroy(row.gameObject);
        }
        rows.Clear();
        AddRow(rootEntry, 0);
        LayoutRebuilder.ForceRebuildLayoutImmediate(treeContent);
        treeScroll.StopMovement();
        treeScroll.verticalNormalizedPosition = scroll;
    }

    void AddRow(EncyclopediaEntry entry, int depth)
    {
        EncyclopediaRow row = Instantiate(rowTemplate, treeContent);
        row.gameObject.SetActive(true);
        row.Configure(entry, depth, expanded.Contains(entry), entry == selected, () => SelectRow(entry));
        rows.Add(row);
        if (expanded.Contains(entry))
        {
            foreach (EncyclopediaEntry child in entry.Children)
            {
                AddRow(child, depth + 1);
            }
        }
    }

    void SelectRow(EncyclopediaEntry entry)
    {
        if (entry.Children.Count > 0)
        {
            if (!expanded.Add(entry))
            {
                expanded.Remove(entry);
            }
        }
        selected = entry;
        ShowArticle();
        RebuildTree();
    }

    void OnDisable()
    {
        Close();
    }

}
