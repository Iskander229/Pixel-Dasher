using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IItem ITEM = collision.GetComponent<IItem>(); //get "interface item" script component from collided object

        if(ITEM != null) //if object didn't have this interface means it will be null and not "collected" (destroyed in their code)
        {
            ITEM.Collect(); //collect object
        }
    }
}
