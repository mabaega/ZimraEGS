namespace Zimra.ApiClient.Helpers
{
    public readonly struct ObjectResponseResult<T>(T responseObject, string responseText)
    {
        public T Object { get; } = responseObject;

        public string Text { get; } = responseText;
    }

}

