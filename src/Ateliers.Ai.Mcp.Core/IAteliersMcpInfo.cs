namespace Ateliers.Ai.Mcp.Core
{
    /// <summary>
    /// MCPのアセンブリ情報を提供するインターフェイス
    /// </summary>
    public interface IAteliersMcpInfo
    {
        /// <summary>
        /// バージョン情報
        /// </summary>
        public Version AssemblyVersion { get; }

        /// <summary>
        /// ライブラリ名
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 会社名
        /// </summary>
        public string Company { get; }

        /// <summary>
        /// 製品名
        /// </summary>
        public string Product { get; }

        /// <summary>
        /// リポジトリURL
        /// </summary>
        public Uri RepositoryUrl { get; }
    }
}
