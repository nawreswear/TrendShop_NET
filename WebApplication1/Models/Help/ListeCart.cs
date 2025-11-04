using System;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Models.Help
{
    public class ListeCart
    {
        public List<Item> Items { get; private set; }

        public static readonly ListeCart Instance;

        static ListeCart()
        {
            Instance = new ListeCart();
            Instance.Items = new List<Item>();
        }

        protected ListeCart() { }

        // Add a product to the cart
        public void AddItem(Product prod)
        {
            if (prod == null) return;

            foreach (Item a in Items)
            {
                if (a.Prod != null && a.Prod.ProductId == prod.ProductId)
                {
                    a.quantite++;
                    return;
                }
            }

            Item newItem = new Item(prod);
            newItem.quantite = 1;
            Items.Add(newItem);
        }

        // Reduce quantity by one
        public void SetLessOneItem(Product prod)
        {
            if (prod == null) return;

            foreach (Item a in Items)
            {
                if (a.Prod != null && a.Prod.ProductId == prod.ProductId)
                {
                    if (a.quantite <= 1)
                    {
                        RemoveItem(a.Prod);
                        return;
                    }
                    else
                    {
                        a.quantite--;
                        return;
                    }
                }
            }
        }

        // Set a specific quantity
        public void SetItemQuantity(Product prod, int quantity)
        {
            if (prod == null) return;

            if (quantity <= 0)
            {
                RemoveItem(prod);
                return;
            }

            foreach (Item a in Items)
            {
                if (a.Prod != null && a.Prod.ProductId == prod.ProductId)
                {
                    a.quantite = quantity;
                    return;
                }
            }
        }

        // Remove a product from the cart
        public void RemoveItem(Product prod)
        {
            if (prod == null) return;

            Item itemToRemove = null;

            foreach (Item a in Items)
            {
                if (a.Prod != null && a.Prod.ProductId == prod.ProductId)
                {
                    itemToRemove = a;
                    break;
                }
            }

            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }

        // Get total price of the cart
        public double GetSubTotal()
        {
            double subTotal = 0;

            foreach (Item i in Items)
            {
                subTotal += i.TotalPrice;
            }

            return subTotal;
        }
    }
}

