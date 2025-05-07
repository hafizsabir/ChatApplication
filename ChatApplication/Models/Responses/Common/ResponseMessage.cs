using ChatApplication.Models.Responses.Common;

public readonly struct ResponseMessage
{
    public ResponseMessages EnumValue { get; }

    public ResponseMessage(ResponseMessages value)
    {
        EnumValue = value;
    }

    // Override ToString to return the description directly
    public override string ToString()
    {
        return EnumValue.GetDescription();  // Use GetDescription for string representation
    }

    // Implicitly convert ResponseMessage to string
    public static implicit operator string(ResponseMessage message) => message.ToString();

    // Implicitly convert ResponseMessages enum to ResponseMessage
    public static implicit operator ResponseMessage(ResponseMessages value) => new ResponseMessage(value);

    // Implicitly convert string to ResponseMessage
    public static implicit operator ResponseMessage(string value)
    {
        return new ResponseMessage(Enum.TryParse<ResponseMessages>(value, out var result) ? result : ResponseMessages.UnknownError);
    }

    // Implicitly convert ResponseMessage back to ResponseMessages enum
    public static implicit operator ResponseMessages(ResponseMessage message) => message.EnumValue;
}
