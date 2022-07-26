using System;
using WallaShops.Utils;

namespace WSOrderCreator.Model
{
  public class ShopperDetails
  {
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName { get; set; }

    public string Idz { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public DateTime Birthdate { get; set; }

    public ShopperDetails SetNames()
    {
      string[] fullName = splitStrToTwoByDelimiterFirstAppearance(this.FullName, delimiter: ' ');

      this.FirstName = fullName[0];
      this.LastName = fullName.Length > 1 ? fullName[1] : WSGeneralUtils.GetAppSettings("DefaultShopperLastName");

      return this;
    }

    public ShopperDetails ValidateIdz()
    {
      bool isIdzValid = WSValidationUtils.ValidateIDNumber(this.Idz.ToString()) == WSValidationUtilsErrors.Valid;

      if (!isIdzValid)
      {
        this.Idz = WSGeneralUtils.GetAppSettings("DefaultShopperIdz");
      }

      return this;
    }

    private string[] splitStrToTwoByDelimiterFirstAppearance(string str, char delimiter)
      =>
      str.Split(new[] { delimiter }, 2);
  }
}
