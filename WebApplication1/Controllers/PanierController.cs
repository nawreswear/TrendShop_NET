using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Text.Json;
using WebApplication1.Models;
using WebApplication1.Models.Help;
using WebApplication1.Models.Repositories;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class PanierController : Controller
    {
        readonly IProductRepository productRepository;
        readonly IOrderRepository orderRepository;
        private readonly UserManager<IdentityUser> userManager;

        public PanierController(IProductRepository productRepository,
        IOrderRepository orderRepository,
        UserManager<IdentityUser> userManager)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.userManager = userManager;
        }

        public ActionResult Index()
        {
            ViewBag.Liste = ListeCart.Instance.Items;
            ViewBag.total = ListeCart.Instance.GetSubTotal();
            return View();
        }

        // Votre méthode AddProduct existante reste inchangée
        public IActionResult AddProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.AddItem(pp);
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        public ActionResult PlusProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.AddItem(pp);
            Item trouve = null;
            foreach (Item a in ListeCart.Instance.Items)
            {
                if (a.Prod.ProductId == pp.ProductId)
                    trouve = a;
            }
            var results = new
            {
                ct = 1,
                Total = ListeCart.Instance.GetSubTotal(),
                Quatite = trouve.quantite,
                TotalRow = trouve.TotalPrice
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult MinusProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.SetLessOneItem(pp);
            Item trouve = null;
            foreach (Item a in ListeCart.Instance.Items)
            {
                if (a.Prod.ProductId == pp.ProductId)
                    trouve = a;
            }
            if (trouve != null)
            {
                var results = new
                {
                    Total = ListeCart.Instance.GetSubTotal(),
                    Quatite = trouve.quantite,
                    TotalRow = trouve.TotalPrice,
                    ct = 1
                };
                return Json(results);
            }
            else
            {
                var results = new
                {
                    ct = 0
                };
                return Json(results);
            }
        }

        [HttpPost]
        public ActionResult RemoveProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.RemoveItem(pp);
            var results = new
            {
                Total = ListeCart.Instance.GetSubTotal(),
            };
            return Json(results);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
            }
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
            }
            var cartItems = ListeCart.Instance.Items.ToList();
            var totalAmount = ListeCart.Instance.GetSubTotal();
            var viewModel = new OrderViewModel
            {
                CartItems = cartItems.Select(item => new CartItemViewModel
                {
                    ProductName = item.Prod.Name,
                    Quantity = item.quantite,
                    Price = item.Prod.Price
                }).ToList(),
                TotalAmount = (float)totalAmount
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.GetUserAsync(User).Result;
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Utilisateur non authentifié.";
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
                }

                var cartItems = ListeCart.Instance.Items.ToList();
                model.CartItems = cartItems.Select(item => new CartItemViewModel
                {
                    ProductName = item.Prod.Name,
                    Quantity = item.quantite,
                    Price = item.Prod.Price
                }).ToList();
                model.TotalAmount = (float)ListeCart.Instance.GetSubTotal();

                var order = new Order
                {
                    CustomerName = user.UserName,
                    Email = user.Email,
                    Address = model.Address,
                    TotalAmount = model.TotalAmount,
                    OrderDate = DateTime.Now,
                    UserId = user.Id,
                    Items = model.CartItems.Select(item => new OrderItem
                    {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                };

                orderRepository.Add(order);
                ListeCart.Instance.Items.Clear();
                TempData["SuccessMessage"] = "Votre commande a été passée avec succès.";
                return RedirectToAction("Confirmation", new { orderId = order.Id });
            }
            TempData["ErrorMessage"] = "Une erreur est survenue. Veuillez vérifier les informations.";
            return View(model);
        }

        public IActionResult Confirmation(int orderId)
        {
            var order = orderRepository.GetById(orderId);
            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
                return RedirectToAction("Index", "Home");

            var service = new SessionService();
            Session session = service.Get(session_id);

            // Récupérer les détails du client
            var customerEmail = session.CustomerDetails?.Email ?? "Client";

            // Si l'utilisateur est connecté, on récupère son identité
            IdentityUser? user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = await userManager.GetUserAsync(User);
            }

            // Créer la commande à partir du panier existant
            var cartItems = ListeCart.Instance.Items.ToList();
            var orderItems = cartItems.Select(item => new OrderItem
            {
                ProductName = item.Prod.Name,
                Quantity = item.quantite,
                Price = item.Prod.Price
            }).ToList();

            var order = new Order
            {
                CustomerName = user?.UserName ?? customerEmail,
                Email = user?.Email ?? customerEmail,
                Address = session.CustomerDetails?.Address?.Line1 ?? "",
                TotalAmount = (float)(session.AmountTotal / 100m),
                OrderDate = DateTime.Now,
               // PaymentStatus = session.PaymentStatus,
                UserId = user?.Id,
                Items = orderItems
            };

            // Enregistrer via le repository
            orderRepository.Add(order);

            // Vider le panier après la commande
            ListeCart.Instance.Items.Clear();

            // Rediriger vers la page de confirmation
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        // NOUVELLES MÉTHODES AJOUTÉES POUR LA GESTION DU PANIER

        private List<Dictionary<string, object>> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("CartItems");
            var cartList = new List<Dictionary<string, object>>();

            if (!string.IsNullOrEmpty(cartJson))
            {
                try
                {
                    using (var doc = JsonDocument.Parse(cartJson))
                    {
                        if (doc.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in doc.RootElement.EnumerateArray())
                            {
                                var cartItem = new Dictionary<string, object>();
                                foreach (var prop in item.EnumerateObject())
                                {
                                    cartItem[prop.Name] = prop.Value.GetRawText();
                                }
                                cartList.Add(cartItem);
                            }
                        }
                    }
                }
                catch { }
            }

            return cartList;
        }

        private void SaveCartToSession(List<Dictionary<string, object>> items)
        {
            var jsonArray = JsonSerializer.Serialize(items.Select(x => new { productId = x.ContainsKey("productId") ? x["productId"] : 0, addedAt = DateTime.Now }).ToList());
            HttpContext.Session.SetString("CartItems", jsonArray);
        }

        [HttpGet("Panier/GetCartCount")]
        public JsonResult GetCartCount()
        {
            try
            {
                // Utilisez votre ListeCart.Instance pour obtenir le compte
                int count = ListeCart.Instance.Items.Count;

                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCartCount error: {ex.Message}");
                return Json(new { success = true, count = 0 });
            }
        }

        // Méthode alternative si vous voulez utiliser la session au lieu de ListeCart
        [HttpGet("Panier/GetCartCountFromSession")]
        public JsonResult GetCartCountFromSession()
        {
            try
            {
                var cartJson = HttpContext.Session.GetString("CartItems");
                int count = 0;

                if (!string.IsNullOrEmpty(cartJson))
                {
                    try
                    {
                        using (var doc = JsonDocument.Parse(cartJson))
                        {
                            if (doc.RootElement.ValueKind == JsonValueKind.Array)
                            {
                                count = doc.RootElement.GetArrayLength();
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON parse error: {ex.Message}");
                    }
                }

                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCartCount error: {ex.Message}");
                return Json(new { success = true, count = 0 });
            }
        }

        // Méthode alternative pour ajouter un produit avec session (optionnelle)
        [HttpGet]
        public IActionResult AddProductWithSession(int id)
        {
            try
            {
                var cart = GetCartFromSession();

                var newItem = new Dictionary<string, object>
                {
                    { "productId", id },
                    { "addedAt", DateTime.Now }
                };
                cart.Add(newItem);

                SaveCartToSession(cart);

                // Ajoutez aussi à votre ListeCart existant pour maintenir la compatibilité
                Product pp = productRepository.GetById(id);
                ListeCart.Instance.AddItem(pp);

                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddProductWithSession error: {ex.Message}");
                return RedirectToAction("Index", "Product");
            }
        }

    }
}