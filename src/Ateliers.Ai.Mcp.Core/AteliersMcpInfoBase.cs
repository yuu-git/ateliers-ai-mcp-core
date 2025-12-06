using System.Reflection;

namespace Ateliers.Ai.Mcp;

/// <summary>
/// IAteliersMcpInfo の基底実装
/// </summary>
public abstract class AteliersMcpInfoBase : IAteliersMcpInfo
{
    private readonly Assembly _assembly;

    /// <summary>
    /// コンストラクタ（派生クラスのアセンブリを自動取得）
    /// </summary>
    protected AteliersMcpInfoBase()
    {
        _assembly = Assembly.GetCallingAssembly();
    }

    /// <inheritdoc/>
    public Version AssemblyVersion => GetAssemblyVersion();

    /// <inheritdoc/>
    public string AssemblyName => GetAssemblyName();

    /// <inheritdoc/>
    public string Description => GetDescription();

    /// <inheritdoc/>
    public string Company => GetCompany();

    /// <inheritdoc/>
    public string Product => GetProduct();

    /// <inheritdoc/>
    public Uri RepositoryUrl => GetRepositoryUrl();

    private Version GetAssemblyVersion()
    {
        var versionAttribute = _assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (versionAttribute != null && Version.TryParse(versionAttribute.InformationalVersion, out var version))
        {
            return version;
        }
        return _assembly.GetName().Version ?? new Version(0, 0, 0, 0);
    }

    private string GetAssemblyName()
    {
        return _assembly.GetName().Name ?? "Unknown";
    }

    private string GetDescription()
    {
        return _assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "";
    }

    private string GetCompany()
    {
        return _assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "";
    }

    private string GetProduct()
    {
        return _assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "";
    }

    private Uri GetRepositoryUrl()
    {
        var metadataAttribute = _assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(x => x.Key == "RepositoryUrl");
        return metadataAttribute != null && Uri.TryCreate(metadataAttribute.Value, UriKind.Absolute, out var uri)
            ? uri
            : new Uri("about:blank");
    }
}