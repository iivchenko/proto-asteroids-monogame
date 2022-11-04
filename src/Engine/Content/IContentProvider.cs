using System.Collections.Generic;

namespace Engine.Content
{
    public interface IContentProvider
    {
        TContent Load<TContent>(string path)
            where TContent : ContentObject;

        IEnumerable<string> GetFiles(string subFolder);

    }
}
