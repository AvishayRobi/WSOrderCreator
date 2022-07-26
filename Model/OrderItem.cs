using WallaShops.Objects;

namespace WSOrderCreator.Model
{
  public class OrderItem
  {
    public int Quantity { get; set; }

    public int FinalPrice { get; set; }

    public int PriceBeforeDiscount { get; set; }

    public int CouponAmount { get; set; }

    public string Description { get; set; }

    public int AuctionID { get; set; }

    public IWSAuction AuctionDetails { get; set; }

    public int ShopID { get; set; }

    public OrderItem ValidatePrices()
    {
      bool isPricesValid = this.PriceBeforeDiscount <= this.FinalPrice;

      if (!isPricesValid)
      {
        this.PriceBeforeDiscount = this.FinalPrice;
      }

      return this;
    }
  }
}
