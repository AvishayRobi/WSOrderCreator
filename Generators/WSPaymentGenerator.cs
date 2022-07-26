using WallaShops.Objects;

namespace WSOrderCreator.Generators
{
  public class WSPaymentGenerator
  {
    public WSPayment GetPayment()
      =>
      new WSPayment(methodType: WSPaymentMethodsType.Manual);
  }
}
