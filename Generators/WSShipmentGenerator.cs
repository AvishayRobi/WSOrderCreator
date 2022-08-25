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

    public WSShipmentGenerator GenerateFromCasualOrder(CasualOrder casualOrder, WSOrderProcess orderProcess)
    {
      this.generatedShipment = getInitializedShipment(casualOrder, orderProcess);
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
      bool isOrderContainsShipments = this.wsOrder
        .OrderShipments
        .Any();

      if (isOrderContainsShipments)
      {
        addItemsToShipment();
      }
    }

    private void addItemsToShipment()
      =>
      this.wsOrder
      .Items
      .Values
      .ApplyEach(i =>
      {
        this.wsOrder
        .OrderShipments
        .Values
        .ElementAt(0)
        .TryToAddItemToShipment(this.wsOrder, i);
      });

    private WSShipment getInitializedShipment(CasualOrder casualOrder, WSOrderProcess orderProcess)
      =>
      new WSShipment(
        shippingMethod: WSShipmentMethodWizard.GetShipmentMethod(casualOrder),
        recipientContact: getContactDetails(casualOrder.ShopperDetails),
        recipientAddress: getAddress(casualOrder.ShippingDetails),
        createdByProcess: orderProcess,
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
  }
}
