namespace Core.Constants
{
    public class Messages
    {
        #region Crud Entity
        public const string CreateSuccess = "Entity created successfully";
        public const string CreateError = "Error occurred while registering the entity to Database";

        public const string DeleteSuccess = "Entity deleted successfully";
        public const string DeleteError = "Error occurred while deleting the entity";

        public const string UpdateSuccess = "Entity updated successfully";
        public const string UpdateError = "Error occurred while updating the entity";
        #endregion

        #region Not Found 
        public const string UserNotFound = "User not found";
        public const string CategoryNotFound = "Category not found";
        public const string ProductNotFound = "Product not found";
        public const string CustomerNotFound = "Customer not found";
        public const string SellerNotFound = "Seller not found";
        public const string ShopNotFound = "Shop not found";
        public const string OrderNotFound = "Order not found";
        public const string OrderItemNotFound = "OrderItem not found";
        #endregion

        #region Empty List
        public const string EmptyEntityList = "The list is empty with the current filter";
        #endregion

        #region Already Exist
        public const string AlreadyExists = "Entity is already exists";
        public const string AlreadyExistsTaxNumber = "TaxNumber has been used before";
        public const string AlreadyExistsEmail = "Email is already exists";
        public const string AlreadyExistsShopName = "ShopName is already exists";
        public const string AlreadyExistsPhone = "Phone number has been used before";
        public const string AlreadyExistsSeller = "Seller is already exists";
        public const string AlreadyExistsShop = "Shop is already exists";
        public const string AlreadyExistsCustomer = "Customer is already exists";
        #endregion

        #region NoChanges
        public const string NoChangesDetected = "You haven't made any changes";
        #endregion

        #region Login / Logout Messages
        public const string LoginSuccess = "Login successfull.";
        public const string LoginInvalidCredentials = "Email or password is incorrect.";
        public const string LogoutSuccess = "Logout successfull.";
        public const string LoginUnauthorized = "Please log in to perform this operation.";

        #endregion

        #region Registration
        public const string RegisterSellerSuccess = "Your application has been received. After review, you will be contacted via e-mail";
        public const string RegisterCustomerSuccess = "You have successfully registered";
        public const string AlreadyRegistered = "This email is already registered";
        #endregion

        #region Password
        public const string PasswordChangeSuccess = "Password changed successfully.";
        public const string OldPasswordError = "Old password is wrong";
        #endregion

        #region Seller
        public const string InvalidStatus = "This user's status is not suitable for the action you are trying to perform.";
        public const string SellerApproved = "Seller successfully approved.";
        public const string SellerRejected= "Seller successfully rejected.";
        public const string SellerBanned = "Seller successfully banned.";

        #endregion

        #region Product Messages 

        public const string ProductAddedSuccess = "Product added to cart";
        public const string ProductAddedError = "Error occurred while adding the Product to Cart";
        public const string DeactivateProductSuccess = "Product has been successfully deactivated";
        public const string DeactivateProductError = "Error occurred while deactivating Product";
        public const string ProductAlreadyInactive = "Product is already inactive.";
        public const string ReactivateProductError = "Error occurred while reactivating Product";
        public const string ReactivateProductSuccess = "Product has been successfully reactivated";
        public const string ProductAlreadyActive = "Product is already active.";
        public const string DeleteProductError = "You can only delete products that are in soldout status";
        public const string InsufficientStock = "Product stock is insufficient";
        public const string NoProductFilters = "No products found matching the filters";
        public const string ProductUnauthorized = "You are not allowed to modify this product.";
        #endregion

        #region Category Messages
        public const string EmptyProductListForCategoryError = "This category does not have any products";
        #endregion

        #region General
        public const string NoFilter = "Please enter at least one filter ";

        #endregion



        #region User
        public const string UnauthorizedAccess = "You do not have authorization to perform this action";
        public const string UserNotAuthenticated = "User not authenticated ";

        #endregion


        #region Shop Messages
        public const string ShopSellerMismatch = "Shop does not belong to this seller";
        public const string ShopAlreadyActive = "Shop is already active";
        public const string ShopAlreadyInactive = "Shop is already inactive.";
        public const string ShopAlreadyDeleted = "Shop is already deleted.";
        public const string ShopAlreadyProcessed = "Shop has already been processed";

        public const string ApprovedSuccess = "Shop has been successfully approved";
        public const string RejectedSuccess = "Shop has been successfully rejected";
        public const string DeactivateShopSuccess = "Shop has been successfully deactivated";
        public const string ReactivateSuccess = "Shop has been successfully reactivated";
        public const string NotAllowedDelete = "You can only delete inactive shops";

        #endregion



        #region OrderItem
        public const string LessCountItemError = "Quantity cannot be less than 1.";
        public const string ExceedCountItemError = "Quantity cannot exceed 100.";
        public const string StockError= "The product quantity you entered is not in stock.";
        #endregion


        #region Order
        public const string OrderStatusIsNotPending = "Order is not in Pending status";
        public const string OrderMarkAsShippedSuccess = "Order marked as shipped successfully";
        public const string OrderMarkAsShippedError = "Error occurred while marking the order as shipped";
        public const string CustomerEmailError = "customer e-mail not found";
        #endregion

    }


    public static class LogMessages
    {
        public const string EmailFailed = "Shop approved but email failed to send: {Email}";
    }


}
