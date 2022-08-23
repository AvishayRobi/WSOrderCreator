using System;
using System.Collections.Generic;
using WallaShops.Objects;
using WallaShops.Utils;
using WSOrderCreator.Model;

namespace WSOrderCreator.Generators
{
  public class WSOrderItemsGenerator
  {
    #region Data Members
    private WSAuctionsFactory auctionItemFactory { get; set; }
    private WSSalesFactory salesFactory { get; set; }
    private string shopperID { get; set; }
    private WSOrder wsOrder { get; set; }
    #endregion

    #region Ctor
    public WSOrderItemsGenerator()
    {
      this.auctionItemFactory = new WSAuctionsFactory();
      this.salesFactory = new WSSalesFactory();
      this.shopperID = string.Empty;
    }
    #endregion

    public WSOrderItemsGenerator SetOrder(WSOrder order)
    {
      this.wsOrder = order;

      return this;
    }

    public WSOrderItemsGenerator SetShopperId(string shopperID)
    {
      this.shopperID = shopperID;

      return this;
    }

    public void GenerateOrderItems(List<OrderItem> orderItems)
      =>
      orderItems
      .ForEach(i =>
      {
        RepeatAction(i.Quantity, () => setUpOrderItem(i));
      });

    private void setUpOrderItem(OrderItem orderItem)
    {
      int auctionItemId = addAuctionItem(orderItem);
      handleCouponItem(orderItem);
      handleDiscount(orderItem, auctionItemId);
    }

    private void handleCouponItem(OrderItem orderItem)
    {
      bool isProductWithCoupon = orderItem.CouponAmount != 0;

      if (isProductWithCoupon)
      {
        addCouponItem(orderItem.FinalPrice);
      }
    }

    private int addAuctionItem(OrderItem item)
    {
      WSBaseAuction auction = getAuctionDetails(item.AuctionID.ToString());
      WSOrderItem wsOrderItem = getWsOrderItem(item, auction);

      this.wsOrder.AddItem(wsOrderItem);

      return wsOrderItem.ItemId;
    }

    private void handleDiscount(OrderItem item, int auctionItemId)
    {
      bool isItemWithDiscount = item.FinalPrice < item.PriceBeforeDiscount;

      if (isItemWithDiscount)
      {
        WSOrderDiscountItem discountItem = getDiscountItem(item, auctionItemId);
        this.wsOrder.AddItem(discountItem);
      }
    }

    private void addCouponItem(int couponAmount)
    {
      string saleName = WSGeneralUtils.GetAppSettings("SaleName");
      int saleId = WSGeneralUtils.GetAppSettingsInt("SaleId");

      int newCouponId = this.salesFactory.CreatePersonalCoupon(
        coupon_type: (int)WSPersonalCouponTypes.CashCoupon,
        to_date: DateTime.Now.AddDays(2),
        order_id_show: string.Empty,
        coupon_amount: couponAmount,
        shopper_id: this.shopperID,
        from_date: DateTime.Now,
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

      this.wsOrder.AddItem(couponOrderItem);
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

    private WSOrderItem getWsOrderItem(OrderItem item, WSBaseAuction auction)
      =>
      auction.AuctionType == WSAuctionTypes.GroupDeal
      ? getGroupDealItem(item, auction)
      : getProductAuctionItem(item, auction);

    private WSOrderProductGroupDealItem getGroupDealItem(OrderItem item, WSBaseAuction auction)
      =>
      new WSOrderProductGroupDealItem(
        itemBeforeDiscountPrice: item.PriceBeforeDiscount * 100,
        price: item.PriceBeforeDiscount * 100,
        auction: (WSGroupDealAuction)auction);

    private WSOrderProductAuctionItem getProductAuctionItem(OrderItem item, WSBaseAuction auction)
      =>
      new WSOrderProductAuctionItem(
        itemBeforeDiscountPrice: item.PriceBeforeDiscount * 100,
        stockPfId: getAttributeStockPfIdIfExist(item.PfId),
        price: item.PriceBeforeDiscount * 100,
        itemType: WSOrderItemType.Product,
        itemDesc: item.Description,
        relatedItems: null,
        auction: auction);

    private List<WSOrderRelatedItem> getRelatedItems(int auctionItemId)
      =>
      new List<WSOrderRelatedItem>()
      {
        new WSOrderRelatedItem(
          relationType: WSOrderItemRelationType.Parent,
          itemId: auctionItemId)
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
