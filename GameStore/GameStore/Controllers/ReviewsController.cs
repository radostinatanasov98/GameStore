namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Infrastructure;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using GameStore.Services.Clients;
    using GameStore.Services.Games;
    using GameStore.Services.Reviews;
    using GameStore.Services.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class ReviewsController : Controller
    {
        private readonly IReviewService reviewService;
        private readonly IUserService userService;
        private readonly IClientService clientService;
        private readonly IGamesService gamesService;

        public ReviewsController(GameStoreDbContext data)
        {
            this.reviewService = new ReviewService(data);
            this.userService = new UserService(data);
            this.clientService = new ClientService(data);
            this.gamesService = new GamesService(data);
        }

        public IActionResult All(int gameId)
        {
            bool isAdmin = false;
            int clientId = -1;

            if (this.User.Identity.IsAuthenticated)
            {
                isAdmin = this.User.IsAdmin();
                if (this.userService.IsUserClient(this.User.GetId()))
                {
                    clientId = this.clientService.GetClientId(this.User.GetId());
                }
            }

            var reviews = this.reviewService.GetReviewsForViewModel(isAdmin, clientId);

            var model = this.reviewService.GetAllReviewsModel(reviews,
                clientId,
                gameId,
                this.gamesService.GetGameById(gameId).Name);

            return View(model);
        }

        [Authorize]
        public IActionResult Write(int gameId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");

            var clientId = this.clientService.GetClientByUserId(this.User.GetId()).Id;

            if (!this.clientService.ClientOwnsGame(clientId, gameId)) return Redirect("/Home/Error");

            return View(new PostReviewFormModel
            {
                Ratings = new List<int> { 1, 2, 3, 4, 5 }
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Write(int gameId, PostReviewFormModel model)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");

            if ((model.Caption == null && model.Content != null) || (model.Caption != null && model.Content == null) || !ModelState.IsValid)
            {
                model.Ratings = new List<int> { 1, 2, 3, 4, 5 };

                return View(model);
            }
            
            var clientId = this.clientService.GetClientByUserId(this.User.GetId()).Id;

            var ownsGame = this.clientService.ClientOwnsGame(clientId, gameId);

            var alreadyReviewed = this.reviewService.HasReviewed(clientId, gameId);

            if (!ownsGame) return Redirect("/Home/Error");

            if (alreadyReviewed) this.reviewService.Edit(clientId, gameId, model);
            else
            {
                this.reviewService.CreateReview(model.Content,
                model.Caption,
                model.Rating,
                clientId,
                gameId);
            }
            
            return Redirect("/Reviews/All?GameId=" + gameId);
        }

        [Authorize]
        public IActionResult Remove(int reviewId, int gameId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");

            var clientId = this.clientService.GetClientId(this.User.GetId());

            if (!this.User.IsAdmin() && !this.reviewService.IsOwner(clientId, reviewId)) return Redirect("/Home/Error");

            this.reviewService.Remove(clientId, gameId);

            return Redirect("/Reviews/All?GameId=" + gameId);
        }
    }
}
