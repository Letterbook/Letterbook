﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Letterbook.Adapter.TimescaleFeeds.Migrations
{
    [DbContext(typeof(FeedsContext))]
    partial class FeedsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Letterbook.Adapter.TimescaleFeeds.EntityModels.TimelinePost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AudienceId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Authority")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<List<TimelineProfile>>("Creators")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("InReplyToId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<string>("Preview")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimelineProfile>("SharedBy")
                        .HasColumnType("jsonb");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<Guid>("ThreadId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AudienceId");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("AudienceId"), "Hash");

                    b.HasIndex("Authority");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Authority"), "Hash");

                    b.HasIndex("Creators");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Creators"), "GIN");

                    b.HasIndex("PostId");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("PostId"), "Hash");

                    b.HasIndex("Time");

                    b.ToTable("Timelines");
                });
#pragma warning restore 612, 618
        }
    }
}
