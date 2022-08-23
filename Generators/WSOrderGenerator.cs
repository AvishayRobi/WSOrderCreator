using System;
using System.Collections.Generic;
using WallaShops.Objects;
using WSOrderCreator.Model;

namespace WSOrderCreator.Generators
{
  public class WSOrderGenerator
  {
    #region Data Members
    private WSOrder generatedOrder { get; set; }
    private WSShopper wsShopper { get; set; }
    #endregion

    #region Ctor
    public WSOrderGenerator(WSShopper shopper)
    {
      this.wsShopper = shopper;
    }
    #endregion

    public WSOrderGenerator ConvertFromCasualOrder(CasualOrder casualOrder)
    {
      initializeOrder(casualOrder.ShopID, casualOrder.EntryID);
      setAffiliate(casualOrder.AffiliateName);
      setUpOrderItems(casualOrder.Items);

      return this;
    }

    public WSOrderGenerator SaveOrder()
    {
      this.generatedOrder.SaveOrder();

      return this;
    }

    public WSOrder GetOrder()
      =>
      this.generatedOrder;

    private WSOrder getInitializedOrder(int externalOrderID, int shopID)
      =>
      new WSOrder(
        createdByProcess: WSOrderProcess.MatanotCC,
        userIp: "0.0.0.0",
        shopper: this.wsShopper,
        shopId: shopID)
      {
        ExternalReferenceId = externalOrderID.ToString(),
        RequestedPickingDate = DateTime.Now,
      };

    private void setUpOrderItems(List<OrderItem> orderItems)
      =>
      new WSOrderItemsGenerator()
      .SetOrder(this.generatedOrder)
      .SetShopperId(this.wsShopper.ShopperID)
      .GenerateOrderItems(orderItems);

    private void initializeOrder(int shopID, int externalOrderID)
    {
      setProperties(shopID);
      this.generatedOrder = getInitializedOrder(externalOrderID, shopID);
    }

    private void setProperties(int shopID)
    {
      this.generatedOrder.OrderShopper.ShopperInvoiceName = this.wsShopper.FullName;
      this.generatedOrder.OrderShopper.ShopperInvoiceIdz = this.wsShopper.IDNumber;
      this.generatedOrder.OrderShopper.ShopperEmail = this.wsShopper.Email;
      this.generatedOrder.IsSalesForce = false;
      this.generatedOrder.ShopId = shopID;
    }    

    private void setAffiliate(string affiliate)
    {
      this.generatedOrder.AffiliateSystem = affiliate;
      this.generatedOrder.AffiliateSource = affiliate;
    }
  }
}
