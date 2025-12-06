using System.Reflection;

namespace Ateliers.Ai.Mcp.Core;

/// <summary>
/// アセンブリ情報
/// </summary>
public class CoreInfo : IAteliersMcpInfo
{
    /// <summary>
    /// 現在のアセンブリ
    /// </summary>
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    /// <inheritdoc/>
    public Version AssemblyVersion { get; } = GetAssemblyVersion();

    /// <inheritdoc/>
    public string AssemblyName { get; } = GetAssemblyName();

    /// <inheritdoc/>
    public string Description { get; } = GetDescription();

    /// <inheritdoc/>
    public string Company { get; } = GetCompany();

    /// <inheritdoc/>
    public string Product { get; } = GetProduct();

    /// <inheritdoc/>
    public Uri RepositoryUrl { get; } = GetRepositoryUrl();

    /// <summary>
    /// アセンブリのバージョンを取得します。
    /// </summary>
    /// <returns> アセンブリバージョン </returns>
    private static Version GetAssemblyVersion()
    {
        var versionAttribute = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (versionAttribute != null && Version.TryParse(versionAttribute.InformationalVersion, out var version))
        {
            return version;
        }
        return Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
    }

    /// <summary>
    /// アセンブリの名前を取得します。
    /// </summary>
    /// <returns> アセンブリ名 </returns>
    private static string GetAssemblyName()
    {
        return Assembly.GetName().Name ?? "Unknown";
    }

    /// <summary>
    /// アセンブリの説明を取得します。
    /// </summary>
    /// <returns> アセンブリ説明 </returns>
    private static string GetDescription()
    {
        return Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "";
    }

    /// <summary>
    /// アセンブリの会社名を取得します。
    /// </summary>
    /// <returns> 会社名 </returns>
    private static string GetCompany()
    {
        return Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "";
    }

    /// <summary>
    /// アセンブリの製品名を取得します。
    /// </summary>
    /// <returns> 製品名 </returns>
    private static string GetProduct()
    {
        return Assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "";
    }

    /// <summary>
    /// アセンブリのリポジトリURLを取得します。
    /// </summary>
    /// <returns> リポジトリURL </returns>
    private static Uri GetRepositoryUrl()
    {
        // .NET 8以降で利用可能
        var metadataAttribute = Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(x => x.Key == "RepositoryUrl");
        return metadataAttribute != null && Uri.TryCreate(metadataAttribute.Value, UriKind.Absolute, out var uri)
            ? uri
            : new Uri("about:blank");
    }
}