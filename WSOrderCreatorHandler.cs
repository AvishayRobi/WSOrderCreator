using WallaShops.Objects;
using WSOrderCreator.Generators;
using WSOrderCreator.Model;

namespace WSOrderCreator
{
  public class WSOrderCreatorHandler
  {
    #region Data Members
    private WSOrderProcess orderProcess { get; set; }
    #endregion

    #region Ctor
    public WSOrderCreatorHandler()
    {
      this.orderProcess = WSOrderProcess.GenericOrderCC;
    }
    #endregion

    public WSOrderCreatorHandler SetOrderProcess(WSOrderProcess orderProcess)
    {
      this.orderProcess = orderProcess;

      return this;
    }

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
      WSShopper wsShopper = getWsShopper(casualOrder.ShopperID);
      WSOrder generatedOrder = generateOrder(wsShopper, casualOrder);

      return generatedOrder;
    }

    private WSPayment getPayment()
      =>
      new WSPaymentGenerator()
      .GetPayment();

    private WSShopper getWsShopper(string shopperID)
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
      new WSOrderGenerator(shopper)
      .ConvertFromCasualOrder(casualOrder, this.orderProcess)
      .SaveOrder()
      .GetOrder();

    private void createShipment(WSOrder wsOrder, CasualOrder casualOrder)
      =>
      new WSShipmentGenerator()
      .SetWsOrder(wsOrder)
      .GenerateFromCasualOrder(casualOrder, this.orderProcess)
      .AddShippingItem()
      .HandleShipmentItems();
  }
}
