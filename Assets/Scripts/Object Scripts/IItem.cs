using UnityEngine;

public interface IItem 
{
    public void Collect(); //every item will implement their own Collect method which will fire when Player collects whatever item, but function will be different.

}
