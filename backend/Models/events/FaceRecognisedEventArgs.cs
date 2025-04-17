namespace backend.Models.events;

public class FaceRecognisedEventArgs : EventArgs
{
    public string PersonName { get; set; }
    public string ImagePath { get; set; }
}