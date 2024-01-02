//interface for doors
public interface IDoor
{
    public bool IsOpen { get; } //is the door open

    public void Open(); //open the door

    public void Close();    //close the door
}
