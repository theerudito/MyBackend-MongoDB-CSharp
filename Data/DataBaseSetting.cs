
namespace MyBackend_MongoDB_CSharp.Data
{
  public class DataBaseSetting
  {
    public string ConnectionString { get; set; } = null!;
    public string MongoDB_Name { get; set; } = null!;
    public string MongoDB_Collection_One { get; set; } = null!;
    public string MongoDB_Collection_Two { get; set; } = null!;
  }
}