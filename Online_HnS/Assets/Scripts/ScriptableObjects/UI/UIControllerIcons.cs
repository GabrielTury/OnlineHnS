using UnityEngine;

[CreateAssetMenu(fileName = "ControllerIcons", menuName = "ControllerIcons")]
public class UIControllerIcons : ScriptableObject
{
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;
    public Sprite leftright;
    public Sprite updown;

    [Space(10)]

    public Sprite up2;
    public Sprite down2;
    public Sprite left2;
    public Sprite right2;
    public Sprite leftright2;
    public Sprite updown2;

    [Space(10)]

    public Sprite a;
    public Sprite b;
    public Sprite x;
    public Sprite y;
    public Sprite escape;
    public Sprite start;

    [Space(10)]

    public Sprite dPadUp;
    public Sprite dPadDown;
    public Sprite dPadLeft;
    public Sprite dPadRight;

    [Space(10)]

    public Sprite rb;
    public Sprite rt;
    public Sprite lb;
    public Sprite lt;
    public Sprite lm; // Analog stick left button
    public Sprite rm; // Analog stick right button
}
