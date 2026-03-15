using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator doorAnimator;
    public Collider playerCollider;

    private bool playerInside = false;
    private bool openAnimationFinished = false;
    private bool shouldClose = false;

    void Start()
    {
        doorAnimator.Play("door_2_closed");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            playerInside = true;
            openAnimationFinished = false;
            shouldClose = false;
            doorAnimator.Play("door_2_open");
        }
    }

    void Update()
    {
        AnimatorStateInfo state = doorAnimator.GetCurrentAnimatorStateInfo(0);

        // After open animation finishes, switch to opened loop if player is still inside
        if (playerInside && !openAnimationFinished)
        {
            if (state.IsName("door_2_open") && state.normalizedTime >= 1f)
            {
                doorAnimator.Play("door_2_opened");
                openAnimationFinished = true;
            }
        }

        // If player left during opening, close after opening finishes
        if (!playerInside && !openAnimationFinished)
        {
            if (state.IsName("door_2_open") && state.normalizedTime >= 1f)
            {
                doorAnimator.Play("door_2_close");
                openAnimationFinished = true; // Prevent further checks
            }
        }

        // Ensure opened animation keeps playing while player stays
        if (playerInside && openAnimationFinished && !state.IsName("door_2_opened"))
        {
            doorAnimator.Play("door_2_opened");
        }

        // When player leaves while opened, start closing after current loop
        if (shouldClose && state.IsName("door_2_opened") && state.normalizedTime >= 1f)
        {
            doorAnimator.Play("door_2_close");
            shouldClose = false;
        }

        // When closing finishes, go to closed
        if (state.IsName("door_2_close") && state.normalizedTime >= 1f)
        {
            doorAnimator.Play("door_2_closed");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == playerCollider)
        {
            playerInside = false;
            shouldClose = true;
        }
    }
}