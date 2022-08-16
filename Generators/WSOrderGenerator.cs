using System;
using System.Collections.Generic;
using WallaShops.Objects;
using WallaShops.Utils;
using WSOrderCreator.Model;

namespace WSOrderCreator.Generators
{
  public class WSOrderGenerator
  {
    #region Data Members
    private WSAuctionsFactory auctionItemFactory { get; set; }
    private WSSalesFactory salesFactory { get; set; }
    private WSOrder generatedOrder { get; set; }
    #endregion

    #region Ctor
    public WSOrderGenerator(WSShopper shopper, int shopID, int externalOrderID)
    {
      this.auctionItemFactory = new WSAuctionsFactory();
      this.salesFactory = new WSSalesFactory();
      this.generatedOrder = getInitializedOrder(shopper, shopID, externalOrderID);
      setProperties(shopper, shopID);
    }
    #endregion

    public WSOrderGenerator SaveOrder()
    {
      this.generatedOrder.SaveOrder();

      return this;
    }

    public WSOrderGenerator SetOrderItems(List<OrderItem> orderItems, string shopperId)
    {
      orderItems.ForEach(i =>
      {
        i.Quantity.Times(() =>
          setUpOrderItem(i, shopperId));
      });

      return this;
    }

    public WSOrderGenerator SetAffiliate(string affiliate)
    {
      this.generatedOrder.AffiliateSystem = affiliate;
      this.generatedOrder.AffiliateSource = affiliate;

      return this;
    }

    public WSOrder GetOrder()
      =>
      this.generatedOrder;

    private void setUpOrderItem(OrderItem orderItem, string shopperId)
    {
      int auctionItemId = addAuctionItem(orderItem);
      handleCouponItem(orderItem, shopperId);
    }

    private void handleCouponItem(OrderItem orderItem, string shopperId)
    {
      bool isProductWithCoupon = orderItem.CouponAmount != 0;

      if (isProductWithCoupon)
      {
        addCouponItem(orderItem.FinalPrice, shopperId);
      }
    }

    private int addAuctionItem(OrderItem item)
    {
      WSBaseAuction auction = getAuctionDetails(item.AuctionID.ToString());
      item.AuctionDetails = auction;

      WSOrderProductAuctionItem orderAuctionItem = new WSOrderProductAuctionItem(
        itemBeforeDiscountPrice: item.PriceBeforeDiscount * 100,
        stockPfId: getAttributeStockPfIdIfExist(item.PfId),
        price: item.PriceBeforeDiscount * 100,
        itemType: WSOrderItemType.Product,
        itemDesc: item.Description,
        relatedItems: null,
        auction: auction);

      this.generatedOrder.AddItem(orderAuctionItem);

      return orderAuctionItem.ItemId;
    }

    private void handleDiscount(OrderItem item, int auctionItemId)
    {
      bool isItemWithDiscount = item.FinalPrice < item.PriceBeforeDiscount;

      if (isItemWithDiscount)
      {
        WSOrderDiscountItem discountItem = getDiscountItem(item, auctionItemId);
        this.generatedOrder.AddItem(discountItem);
      }
    }

    private void addCouponItem(int couponAmount, string shopperId)
    {
      int saleId = int.Parse(WSGeneralUtils.GetAppSettings("SaleId"));
      string saleName = WSGeneralUtils.GetAppSettings("SaleName");

      int newCouponId = this.salesFactory.CreatePersonalCoupon(
        coupon_type: (int)WSPersonalCouponTypes.CashCoupon,
        to_date: DateTime.Now.AddDays(2),
        order_id_show: string.Empty,
        coupon_amount: couponAmount,
        from_date: DateTime.Now,
        shopper_id: shopperId,
        comments: saleName,
        sale_id: saleId,
        reason_id: 17);

      WSOrderCouponItem couponOrderItem = new WSOrderCouponItem(
        personalCouponType: WSPersonalCouponTypes.CashCoupon,
        couponType: WSCouponTypes.Personal,
        itemPrice: couponAmount * 100,
        couponCode: string.Empty,
        couponId: newCouponId,
        couponSaleId: saleId,
        itemDesc: saleName);

      this.generatedOrder.AddItem(couponOrderItem);
    }

    private void setProperties(WSShopper shopper, int shopId)
    {
      this.generatedOrder.OrderShopper.ShopperInvoiceName = shopper.FullName;
      this.generatedOrder.OrderShopper.ShopperInvoiceIdz = shopper.IDNumber;
      this.generatedOrder.OrderShopper.ShopperEmail = shopper.Email;
      this.generatedOrder.IsSalesForce = false;
      this.generatedOrder.ShopId = shopId;
    }

    private WSOrderDiscountItem getDiscountItem(OrderItem item, int auctionItemId)
      =>
      new WSOrderDiscountItem(
        itemPrice: (item.PriceBeforeDiscount - item.FinalPrice) * 100,
        relatedItems: getRelatedItems(auctionItemId),
        discountType: WSOrderDiscountItemType.Walla,
        reason: WSDiscountReasonType.General,
        itemDesc: "הנחת קופה",
        notes: "פריט הנחה");

    private List<WSOrderRelatedItem> getRelatedItems(int auctionItemId)
      =>
      new List<WSOrderRelatedItem>()
      {
        new WSOrderRelatedItem(
          relationType: WSOrderItemRelationType.Parent,
          itemId: auctionItemId)
      };

    private WSOrder getInitializedOrder(WSShopper shopper, int shopID, int externalOrderID)
      =>
      new WSOrder(
        userIp: WSGeneralUtils.GetAppSettings("defaultIp"),
        createdByProcess: WSOrderProcess.MatanotCC,
        shopper: shopper,
        shopId: shopID)
      {
        SalesForceOrderNo = externalOrderID.ToString(),
        RequestedPickingDate = DateTime.Now,
      };

    private WSBaseAuction getAuctionDetails(string auctionID)
      =>
      this.auctionItemFactory
      .GetAuctionByAuctionID(auctionID);
    
    private string getAttributeStockPfIdIfExist(string sfProductId)
      =>
      this.auctionItemFactory
      .GetWallaShopsAttributeStockPfid(sfProductId);
  }
}
