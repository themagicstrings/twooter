using Microsoft.EntityFrameworkCore;
using Entities;
using Models;

namespace Models.Test
{
    public class MinitwitTestContext : MinitwitContext
    {
        public MinitwitTestContext(DbContextOptions<MinitwitContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { user_id = 1, username = "olduser1", email = "olduser1@mail.io", pw_hash = "CB10E796FAC82E4FE345DD4E30B6B6EF940AAD18CEEB6DD55A7C5A4553816159"},
                new User { user_id = 2, username = "olduser2", email = "olduser2@mail.io", pw_hash = "76E2D06976544FB98DEBD8297BEE138A03CD5C5212F3D70797C07D75A476D4CE"},
                new User { user_id = 3, username = "olduser3", email = "olduser3@mail.io", pw_hash = "B91F86D0CE688DF80AB84175AF163E9964CA4E80179F1D2668B4BB67B600D295"},
                new User { user_id = 4, username = "olduser4", email = "olduser4@mail.io", pw_hash = "5CA833F9ED5BC47AEEECA98D6EC5AFE1D4741D4D7DA7C571E037D624BA141388"}
            );
        }
    }
}