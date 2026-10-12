# Menu layout

## Escape menu

The menu canvas is now named EscapeMenuCanvas and starts inactive in both the
prefab and SampleScene. EscapeMenuController is attached to the active UI root,
outside that canvas. It uses the installed Unity Input System:
`Keyboard.current.escapeKey.wasPressedThisFrame` in Update checks each new press.
Update continues to run when Time.timeScale is zero, so Escape can also resume.

Opening activates the entire menu canvas, pauses scaled game time, unlocks and
shows the cursor. Closing restores the previous time scale and cursor state.
The first button is labelled Fortsetzen and closes the menu. Other buttons keep
their existing tutorial text actions. A dark translucent Image on the menu canvas
provides a backdrop and blocks pointer clicks through to the gameplay UI.
Sorting Order 10 puts the menu above the other canvas.

The pause stops scaled time/physics; gameplay scripts that read input directly
in Update still need to respect the pause state if such scripts are added later.
In Unity, focus the Game view, press Escape, then press it again or click
Fortsetzen. The controller's Pause Game checkbox can disable time pausing.
Tools > Validate Escape Menu checks prefab wiring and open/close/resume behavior.

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
