using UnityEngine;

public class DoorKupola : MonoBehaviour
{
    public Animator doorAnimator;
    public Collider playerCollider;

    private bool playerInside = false;
    private bool openAnimationFinished = false;

    void Start()
    {
        doorAnimator.Play("DoorClosed");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            playerInside = true;
            openAnimationFinished = false;
            doorAnimator.Play("DoorOpen");
        }
    }

    void Update()
    {
        AnimatorStateInfo state = doorAnimator.GetCurrentAnimatorStateInfo(0);

        // After open animation finishes, switch to opened loop
        if (playerInside && !openAnimationFinished)
        {
            if (state.IsName("DoorOpen") && state.normalizedTime >= 1f)
            {
                doorAnimator.Play("DoorOpened");
                openAnimationFinished = true;
            }
        }

        // Ensure opened animation keeps playing while player stays
        if (playerInside && openAnimationFinished && !state.IsName("DoorOpened"))
        {
            doorAnimator.Play("DoorOpened");
        }

        // When player leaves, finish closing cycle
        if (!playerInside && state.IsName("DoorClose") && state.normalizedTime >= 1f)
        {
            doorAnimator.Play("DoorClosed");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == playerCollider)
        {
            playerInside = false;
            doorAnimator.Play("DoorClose");
        }
    }
}