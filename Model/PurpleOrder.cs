using WallaShops.Objects;
using WallaShops.Shops.Payment.BL;

namespace WSOrderCreator.Model
{
  public class PurpleOrder
  {
    public WSOrder WsOrder { get; set; }

    public WSSalesForcePayment Payment { get; set; }

    public string ShopperIdz { get; set; }

    public void MakeOrderGreen()
      =>
      this.WsOrder
      .AddOrderDebit(this.Payment, this.WsOrder.TotalBalance, this.ShopperIdz);
  }
}
