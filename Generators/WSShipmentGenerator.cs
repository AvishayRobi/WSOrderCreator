using System.Linq;
using WallaShops.Objects;
using WallaShops.Utils;
using WSOrderCreator.Model;

namespace WSOrderCreator.Generators
{
  public class WSShipmentGenerator
  {
    #region Data Members
    private WSShipment generatedShipment { get; set; }
    private WSOrder wsOrder { get; set; }
    #endregion

    #region Ctor
    public WSShipmentGenerator()
    {
      this.generatedShipment = null;
      this.wsOrder = null;
    }
    #endregion    

    public WSShipmentGenerator SetWsOrder(WSOrder order)
    {
      this.wsOrder = order;

      return this;
    }

    public WSShipmentGenerator InitializeShipment(CasualOrder casualOrder)
    {
      this.generatedShipment = getInitializedShipment(casualOrder);
      this.wsOrder.AddShipment(this.generatedShipment);

      return this;
    }

    public WSShipmentGenerator AddShippingItem()
    {
      WSOrderShippingItem orderShippingItem = getOrderShippingItem();
      this.wsOrder.AddItem(orderShippingItem);
      this.generatedShipment.SetShippingOrderItemId(orderShippingItem.ItemId);

      return this;
    }

    public void HandleShipmentItems()
    {
      bool isOrderContainsShipments = this.wsOrder.OrderShipments.Any();

      if (isOrderContainsShipments)
      {
        addItemsToShipment();
      }
    }

    private void addItemsToShipment()
    {
      foreach (WSOrderItem item in this.wsOrder.Items.Values)
      {
        this.wsOrder.OrderShipments.Values.ElementAt(0).tryToAddItemToShipment(this.wsOrder, item);
      }
    }

    private WSShipment getInitializedShipment(CasualOrder casualOrder)
      =>
      new WSShipment(
        shippingMethod: getShipmentMethod(casualOrder.Items.First().AuctionDetails),
        recipientContact: getContactDetails(casualOrder.ShopperDetails),
        recipientAddress: getAddress(casualOrder.ShippingDetails),
        createdByProcess: WSOrderProcess.MatanotCC,
        orderId: this.wsOrder.OrderId,
        shippingDays: 1)
      {
        PackageEstimatedQuantity = 1,
        PackageQuantity = 1
      };

    private WSContactDetails getContactDetails(ShopperDetails shopperDetails)
      =>
      new WSContactDetails(
        firstName: shopperDetails.FirstName,
        lastName: shopperDetails.LastName,
        phone: shopperDetails.PhoneNumber);

    private WSOrderShippingItem getOrderShippingItem()
      =>
      new WSOrderShippingItem(
        itemDesc: WSStringUtils.GetDescription(this.generatedShipment.ShippingMethod),
        shipmentId: this.generatedShipment.ShipmentId,
        itemPrice: 0);

    private WSAddress getAddress(ShippingDetails shippingDetails)
      =>
      new WSAddress(
        apartment: shippingDetails.Apratment.ToString(),
        floor: shippingDetails.Floor.ToString(),
        streetName: shippingDetails.StreetName,
        entrance: shippingDetails.Enterance,
        house: shippingDetails.StreetNumber,
        zipCode: shippingDetails.ZipCode,
        cityName: shippingDetails.City,
        isPoConfirmed: false,
        poBox: string.Empty);

    private WSShipmentMethodsType getShipmentMethod(IWSAuction auction)
      =>
      WSShipmentMethod
      .GetAvailableShipmentMethods(new[] { auction })
      .First()
      .MethodType;
  }
}
