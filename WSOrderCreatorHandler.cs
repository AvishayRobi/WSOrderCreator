using WallaShops.Objects;
using WallaShops.Utils;
using WSOrderCreator.Generators;
using WSOrderCreator.Model;

namespace WSOrderCreator
{
  public class WSOrderCreatorHandler
  {
    public void CreateWsAuction(ICasualOrderConvertible convertibleOrder)
    {
      CasualOrder casualOrder = convertibleOrder.ConvertToCasualOrder();
      WSOrder wsOrder = getWsOrder(casualOrder);
      WSPayment payment = getPayment();

      createShipment(wsOrder, casualOrder);
      wsOrder.AddOrderDebit(payment, wsOrder.TotalBalance, casualOrder.ShopperDetails.Idz);
    }

    private WSOrder getWsOrder(CasualOrder casualOrder)
    {
      string shopperID = getShopperID();
      WSShopper guestShopper = generateShopper(shopperID);
      WSOrder generatedOrder = generateOrder(guestShopper, casualOrder);

      return generatedOrder;
    }

    private WSPayment getPayment()
      =>
      new WSPaymentGenerator()
      .GetPayment();

    private WSShopper generateShopper(string shopperID)
      =>
      new WSShopperGenerator()
      .SetShopperID(shopperID)
      .GetShopper();

    private WSShopper generateGuestShopper(ShopperDetails shopperDetails)
      =>
      new WSGuestShopperGenerator()
      .InitializeGuestShopper(shopperDetails)
      .SaveShopper()
      .GetGuestShopper();

    private WSOrder generateOrder(WSShopper shopper, CasualOrder casualOrder)
      =>
      new WSOrderGenerator(shopper, casualOrder.ShopID, casualOrder.EntryID)
      .SetOrderItems(casualOrder.Items, shopper.ShopperID)
      .SetAffiliate(casualOrder.AffiliateName)
      .SaveOrder()
      .GetOrder();

    private void createShipment(WSOrder wsOrder, CasualOrder casualOrder)
      =>
      new WSShipmentGenerator()
      .SetWsOrder(wsOrder)
      .InitializeShipment(casualOrder)
      .AddShippingItem()
      .HandleShipmentItems();

    private string getShopperID()
      =>
      WSGeneralUtils
      .GetAppSettings("CasualShopperID");
  }
}
