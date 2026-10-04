using Baitul_Kitab.Models.DTO.UserStore;
using System;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Interfaces
{
    public interface ICartService
    {
        CartDTO GetCart();
        Task<(bool Success, string Message, CartItemDTO? Item, CartDTO Cart)> AddItemAsync(Guid bookId, int quantity = 1);
        CartDTO UpdateQuantity(Guid bookId, int quantity);
        CartDTO RemoveItem(Guid bookId);
        void ClearCart();
        int GetItemCount();
    }
}
