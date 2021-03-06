﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Configuration
{
    internal class UserFollowerConfiguration : IEntityTypeConfiguration<UserFollower>
    {
        public void Configure(EntityTypeBuilder<UserFollower> builder)
        {
            builder.ToTable("user_followers");

            builder.HasKey(x => new { x.FollowerId, x.UserId });

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id");

            builder
                .Property(x => x.FollowerId)
                .HasColumnName("follower_id");
        }
    }
}
