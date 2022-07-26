using WallaShops.Common.Tests;
using WallaShops.Objects;
using WallaShops.Utils;

namespace WSOrderCreator.Generators
{
  public class WSShopperGenerator
  {#region Data Members
    private string shopperID { get; set; }
    #endregion

    #region Ctor
    public WSShopperGenerator()
    {
      this.shopperID = string.Empty;
    }
    #endregion

    public WSShopperGenerator SetShopperID(string shopperID)
    {
      this.shopperID = shopperID;

      return this;
    }

    public WSShopper GetShopper()
      =>
      new WSShoppersFactory()
      .GetShopperByShopperID(this.shopperID);
  }
}
