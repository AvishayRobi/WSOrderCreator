using WallaShops.Common.SalesForce;
using WallaShops.Common.Tests;
using WallaShops.Objects;
using WallaShops.Utils;
using WallaShops.Utils.Cryptography;
using WSOrderCreator.Model;

namespace WSOrderCreator.Generators
{
  public class WSGuestShopperGenerator
  {
    #region Data Members
    private WSShopper generatedGuestShopper { get; set; }
    #endregion

    #region Ctor
    public WSGuestShopperGenerator()
    {
      this.generatedGuestShopper = null;
    }
    #endregion

    public WSGuestShopperGenerator InitializeGuestShopper(ShopperDetails shopperDetails)
    {
      WSCryptor cryptor = new WSCryptor();

      this.generatedGuestShopper = new WSShopper()
      {
        EncrytedIDNumber = cryptor.EncryptData(shopperDetails.Idz),
        SalesForcePlatform = SalesForcePlatforms.WallaShops,
        BirthDate = shopperDetails.Birthdate.ToString(),
        PhoneNumber = shopperDetails.PhoneNumber,
        FirstName = shopperDetails.FirstName,
        LastName = shopperDetails.LastName,
        IDNumber = shopperDetails.Idz,
        Email = shopperDetails.Email,
        SalesForceShopper = false,
        IsSalesForceGuest = true
      };

      return this;
    }

    public WSGuestShopperGenerator SaveShopper()
    {
      this.generatedGuestShopper.SaveToDatabase();
      this.generatedGuestShopper.InsertShopperToGuestTable();
      
      return this;
    }

    public WSShopper GetGuestShopper()
    {
      bool isShopperInitialized = this.generatedGuestShopper != null;

      if (!isShopperInitialized)
      {
        throw new Exception("Guest shopper must be initialized");
      }

      return this.generatedGuestShopper;
    }
  }
}
