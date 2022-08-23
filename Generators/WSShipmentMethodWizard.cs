using System.Linq;
using WallaShops.Objects;
using WSOrderCreator.Model;

namespace WSOrderCreator.Generators
{
  public static class WSShipmentMethodWizard
  {
    public static WSShipmentMethodsType GetShipmentMethod(CasualOrder order)
      =>
      GetShipmentMethod(
        order
        .Items
        .First()
        .AuctionID);

    public static WSShipmentMethodsType GetShipmentMethod(int auctionID)
      =>
      new WSShipmentMethodWizardDalManager()
      .GetShipmentMethod(auctionID);

    public static WSShipmentMethodsType GetShipmentMethod(IWSAuction auction)
      =>
      WSShipmentMethod
      .GetAvailableShipmentMethods(new[] { auction })
      .First()
      .MethodType;
  }
}
