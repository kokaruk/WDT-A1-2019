using System.Diagnostics.CodeAnalysis;

namespace WdtAsrA1.Utils
{
    /// <summary>
    /// user secrets accessor class
    /// based on some code from
    /// https://medium.com/@granthair5/how-to-add-and-use-user-secrets-to-a-net-core-console-app-a0f169a8713f
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class AsrDb
    {
        public string Uid { get; set; }
        public string Password { get; set; }
    }
}