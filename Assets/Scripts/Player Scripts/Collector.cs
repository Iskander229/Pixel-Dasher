using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IItem item = collision.GetComponent<IItem>(); //get "interface item" script component from collided object

        if(item != null) //if object didn't have this interface means it will be null and not "collected" (destroyed in their code)
        {
            item.Collect(); //collect object
        }
    }
}
