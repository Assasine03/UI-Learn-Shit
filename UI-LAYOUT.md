# Menu layout

## Gameplay HUD positioning

The first canvas keeps Scale With Screen Size (1920 × 1080, Expand).
Panel fills its canvas using anchors (0, 0) to (1, 1), zero offsets and scale 1.
The panel's existing visual styling is unchanged.

| Element | Anchors / pivot | Position | Size in reference units |
| --- | --- | --- | --- |
| Image | Top left / top left | (32, -32) | 292 × 126 |
| Image (1) | Top right / top right | (-32, -32) | 292 × 126 |
| Image (2) | Bottom horizontal stretch / bottom center | (0, 0) | Full width × 64 |
| Image (3) | Bottom left / bottom left | (32, 96) | 292 × 126 |
| Maske | Bottom center / bottom center | (0, 96) | 384 × 175 |

The mask now clips a full-size, stretched HealthFill. HealthOutline is a stationary
Image sibling immediately after the mask in the canvas hierarchy. It uses a
transparent ellipse outline sprite matching the mask, so it remains visible even
at zero health. Both share the same bottom-center anchors and dimensions.
The two lower widgets have a 32-unit gap above the 64-unit progress placeholder.
Corners use fixed reference sizes scaled by CanvasScaler; the progress placeholder
and Panel additionally stretch to the actual canvas width. Obsolete positioning
overrides were removed from SampleScene and UI (1) Variant so the prefab settings
take effect. Slider and Scroll View were not edited; no HUD functions were added.
Prefab references and geometric bounds were checked at six landscape/portrait
resolutions; rendered appearance has not been verified in the Unity Game view.

## Health display and U test key

PlayerCharacter lives under the active UI root, outside the switchable canvases.
Character stores Health (0–100, default 100). TakeDamage and SetHealth clamp the
value and notify HealthChanged listeners. HealthBar on Maske subscribes when
enabled and unsubscribes when disabled. It also refreshes on resizing and detects
Play Mode Inspector edits. Its fill position is:

`x = -maskWidth * (1 - Health / 100)`

The fill remains the size of the mask and moves left; the mask hides overflow.
HealthBar explicitly resets the fill's stretch anchors, pivot, sizeDelta (zero)
and local scale (one) before positioning it. This prevents an old explicit width
from being added to the mask width by stretch anchors. Unity validation checks
actual RectTransform dimensions and that the right edge is outside the mask at 0.
At 100/50/0 health, offsets are 0/−192/−384 for the current 384-unit width.
This describes horizontal displacement, not the visible area percentage of an oval.

LebenTest on PlayerCharacter uses the existing Input System and
Keyboard.current.uKey.wasPressedThisFrame to apply 10 damage per new press.
Damage is ignored while Time.timeScale is zero (Escape menu pause).
For testing, focus the Game view and press U repeatedly; 10 presses empty the bar.
Change Character's Health slider during Play Mode to try other values.
Disable or remove LebenTest when replacing the test with gameplay damage.

Validation: actual Character/HealthBar/LebenTest logic was compiled and exercised
with lightweight Unity API stubs (limits, ten damage steps, display events,
input press and pause). Prefab references and outline transparency were checked.
Tools > Validate Health UI provides additional Unity prefab validation.
Actual Unity import, input delivery and Game-view rendering remain unverified.

## Escape menu

The menu canvas is now named EscapeMenuCanvas and starts inactive in both the
prefab and SampleScene. EscapeMenuController is attached to the active UI root,
outside that canvas. It uses the installed Unity Input System:
`Keyboard.current.escapeKey.wasPressedThisFrame` in Update checks each new press.
Update continues to run when Time.timeScale is zero, so Escape can also resume.

Opening activates the entire menu canvas, pauses scaled game time, unlocks and
shows the cursor. Closing restores the previous time scale and cursor state.
The first button is labelled Fortsetzen and closes the menu. Settings opens the
settings window on Spiel. Lobby and Leave Mission have no callbacks yet. Close
Game stops Play Mode in the editor (UNITY_EDITOR) or calls Application.Quit in a
build. The old UIManager tutorial component is disabled so it does not attach
text-changing callbacks to these buttons. A dark translucent Image on the menu canvas
provides a backdrop and blocks pointer clicks through to the gameplay UI.
Sorting Order 10 puts the menu above the other canvas.

The pause stops scaled time/physics; gameplay scripts that read input directly
in Update still need to respect the pause state if such scripts are added later.
In Unity, focus the Game view, press Escape, then press it again or click
Fortsetzen. The controller's Pause Game checkbox can disable time pausing.
Tools > Validate Escape Menu checks prefab wiring and open/close/resume behavior.

## Settings window

MenuColumn has a purple MenuBackground child with stretch anchors and 48 extra
units in both dimensions (24 units on each side). Layout Element > Ignore Layout
is enabled, so this background does not participate in the vertical button layout.
It is the first child, behind the other children. LogoSlot contains a blue Image
with a centered LOGO label as a placeholder.

SettingsWindow is an initially inactive child of EscapeMenuCanvas. It stretches
to the available screen with 392 units of left margin, 32 right, and 64 top/bottom.
Inside it, the four tab buttons share the header width equally. The green content
area stretches between the header and the common footer. The common Zurück button
remains visible on all tabs and closes Settings without resuming the game.

SettingsMenuController on the active UI root owns the settings, tab and back
callbacks. Two ordered arrays pair tabButtons with tabPanels: Spiel, Grafik,
Audio, Sonstiges. SelectTab enables only the corresponding panel and updates
button background and text colors (selected text red, other text white).
OpenSettings always selects index 0. A local tabIndex captures each button's
own index when registering its callback. Tab transitions are disabled so Unity's
hover color does not overwrite the active-tab highlight.

EscapeMenuController closes Settings when the Escape menu is closed or reopened.
The window has no actual settings controls yet, only named placeholders.

Validation: controller source was compiled and exercised against lightweight
Unity API stubs, including each tab callback, highlights, Back from each tab,
reopen on Spiel, resume cleanup and quit callback. Prefab references, parent/child
links and layout bounds at six resolutions were checked. Tools > Validate Settings
Menu provides Unity prefab/callback validation. Unity import, actual mouse input,
Play Mode termination and rendered appearance have not been verified here.

The two screen-space canvases in `Assets/UI.prefab` use **Scale With Screen Size**, a
1920 × 1080 reference resolution, and **Expand**. Both canvases therefore use the
same scale. Expand preserves at least the reference canvas area at other aspect
ratios; on narrow screens the buttons become smaller instead of being cropped.

`MenuColumn` is anchored at the top left with a 32-unit margin and width 274.
Its ordered children are LogoSlot (180 high), LogoGap (40), TopButtonList
(three buttons), GroupGap (65), and BottomButtonList (two buttons).
The column uses a Vertical Layout Group with zero padding and spacing,
Upper Left alignment, controlled child width and height, force expand width
enabled and force expand height disabled. Its Content Size Fitter uses
Horizontal Fit Unconstrained and Vertical Fit Preferred Size.

Each slot/gap has a Layout Element with Min Height and Preferred Height set
to the height above, Flexible Height 0 and Ignore Layout disabled.
Both button lists have a Vertical Layout Group with padding 0, spacing 25,
Upper Left alignment, controlled child width but not height, force expand
width enabled and height disabled. Neither list has a Content Size Fitter:
the column controls their heights using the lists' preferred layout sizes.
List pivots are (0, 1). Positions are controlled by the column's layout.

To reproduce: create an empty UI object MenuColumn under the menu Canvas.
Set Anchor Min/Max and Pivot to (0, 1), Pos X 32, Pos Y -32, Width 274,
Scale (1, 1, 1). Add the layout components with the settings above. Create
the slot/gaps and move both existing lists underneath in the stated order.
Remove the lists' previous Content Size Fitters and preserve button order
and click references. Make these changes in UI.prefab's Prefab Mode.

For the future logo, add an Image child under LogoSlot, stretch it on both
axes with offsets 0, assign the sprite and enable Preserve Aspect.
The menu currently needs 860 units of height, ending 892 units below the
screen top. Adding buttons automatically increases its height; use a
ScrollRect if it eventually exceeds the available canvas height.

`Assets/GameObject.prefab` is the reusable MenuButton. Its default size is
274 × 100. Image, clickable Button, and label stretch to the root rectangle.
Change the list width for all buttons in that list, and the base prefab height
for the standard button height. Existing click handlers and manager references
are preserved.

After importing, check SampleScene in Unity's Game view at 1920 × 1080,
1280 × 720, 1024 × 768, 2560 × 1080, and 1080 × 1920. Check button clicks and
canvas toggling as well. Prefab references and menu bounds were checked from
the saved files; rendered appearance and input behavior require Unity testing.
# ESC backdrop and Settings transparency

EscapeMenuBackdrop enables the live EscapeMenuBlurFeature in both URP renderers.
The two-pass Gaussian blur processes the current camera color each frame before
the Screen Space Overlay canvases are drawn. No screenshot or RawImage is used.
The menu canvas Image provides a separate black overlay at alpha 0.4 (40%
darkening). The gameplay HUD and menu remain sharp. Camera rendering continues
when Time.timeScale is zero, while ordinary game animations remain paused.
The feature uses URP 17 Render Graph (compatibility mode must remain disabled).

The orange Settings Image and all four green content Images use alpha 0.9:
10% transparency. Labels and buttons retain their own opacity.
SettingsWindowFrame replaces the orange Image mesh with four strips around
the Content RectTransform. There is no orange surface underneath green.
Without this cutout, two overlapping alpha-0.9 surfaces would transmit only
0.1 * 0.1 = 1% of the background. The cutout follows the content's anchored
rectangle when the canvas changes size.
