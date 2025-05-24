using UnityEngine;

public static class CustomerAnimatorAnimationHashes
{
    //Idle
    public static readonly int Idle = Animator.StringToHash("Idle");

    //Eye LidsS
    public static readonly int EyeLids_Closed = Animator.StringToHash("Eyes_Closed");
    public static readonly int EyeLids_Open = Animator.StringToHash("Eyes_Open");

    //Eyes Iris'
    public static readonly int Eyes_Neutral = Animator.StringToHash("Eyes_Neutral");
    public static readonly int Eyes_ToLeft = Animator.StringToHash("Eyes_ToLeft");
    public static readonly int Eyes_ToRight = Animator.StringToHash("Eyes_ToRight");

    //Mouth
    public static readonly int Mouth_Open = Animator.StringToHash("Mouth_Open");
    public static readonly int Mouth_Close = Animator.StringToHash("Mouth_Close");

    //Eyebrows
    public static readonly int Eyebrows_Neutral = Animator.StringToHash("Eyebrows_Neutral");
    public static readonly int Eyebrows_Up = Animator.StringToHash("Eyebrows_Up");
    public static readonly int Eyebrows_Down = Animator.StringToHash("Eyebrows_Down");
}


public enum CustomerAnimatorLayers
{
    BaseLayer,
    Mouth,
    Eyes_Movement,
    Eyes_Blinking,
    Eyebrows
}

public enum CostumerMergingStates
{
    Idle,
    Blinking,
    LookingLeft,
    LookingRight,
    MouthOpen,
    MouthClose
}
