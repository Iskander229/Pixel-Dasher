using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"Entered: {collision.name}, ID: {collision.GetInstanceID()}");


        IItem item = collision.GetComponent<IItem>(); //get "interface item" script component from collided object

        if(item != null) //if object didn't have this interface means it will be null and not "collected" (destroyed in their code)
        {
            item.Collect(); //collect object
            //Debug.Log("Player called item's 'Collect' method ");
        }
    }
}
