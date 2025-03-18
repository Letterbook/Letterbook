using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

/// <summary>
/// These configurations are a workaround for a bug in AspnetCore.Identity.EntityFramework
/// These entities are required by AspnetCore Identity. Identity *should* detect the relations and do these
/// configurations. But it just skips that step unless we inherit our dbContext from IdentityDbContext.
/// That makes some sense when you get identities from an external source and you just need to authenticate against them.
/// We're not doing that. We're managing our own identity data, which would be a lot more difficult if it's sectioned
/// off from the rest of the application.
///
/// If the AspnetCore auth team ever gets their shit together, we can likely remove these. I know I would prefer not to
/// maintain these entity configs myself.
///
/// Anyway, these are the configs that Identity.EntityFramework would discover and generate using IdentityDbContext. 
/// See:
/// https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/authentication/customize-identity-model.md#customize-the-model
/// https://github.com/dotnet/aspnetcore/issues/12511
/// </summary>
public class ConfigureIdentityRole : IEntityTypeConfiguration<IdentityRole<Guid>>
{
	public void Configure(EntityTypeBuilder<IdentityRole<Guid>> b)
	{
		b.Property<Guid>("Id")
			.HasColumnType("uuid");

		b.Property<string>("ConcurrencyStamp")
			.IsConcurrencyToken()
			.HasColumnType("text");

		b.Property<string>("Name")
			.HasMaxLength(256)
			.HasColumnType("character varying(256)");

		b.Property<string>("NormalizedName")
			.HasMaxLength(256)
			.HasColumnType("character varying(256)");

		b.HasKey("Id");

		b.HasIndex("NormalizedName")
			.IsUnique()
			.HasDatabaseName("RoleNameIndex");

		b.ToTable("AspNetRoles", "AspnetIdentity");
	}
}

public class ConfigureIdentityRoleClaim : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
	public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> b)
	{
		b.Property<int>("Id")
			.ValueGeneratedOnAdd()
			.HasColumnType("integer");

		NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

		b.Property<string>("ClaimType")
			.HasColumnType("text");

		b.Property<string>("ClaimValue")
			.HasColumnType("text");

		b.Property<Guid>("RoleId")
			.IsRequired()
			.HasColumnType("uuid");

		b.HasKey("Id");

		b.HasIndex("RoleId");

		b.ToTable("AspNetRoleClaims", "AspnetIdentity");

		b.HasOne<IdentityRole<Guid>>() //("Microsoft.AspNetCore.Identity.IdentityRole", null)
			.WithMany()
			.HasForeignKey("RoleId")
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();
	}
}

public class ConfigureIdentityUserClaim : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
	public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> b)
	{
		b.Property<int>("Id")
			.ValueGeneratedOnAdd()
			.HasColumnType("integer");

		NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

		b.Property<string>("ClaimType")
			.HasColumnType("text");

		b.Property<string>("ClaimValue")
			.HasColumnType("text");

		b.Property<Guid>("UserId")
			.IsRequired()
			.HasColumnType("uuid");

		b.HasKey("Id");

		b.HasIndex("UserId");

		b.ToTable("AspNetUserClaims", "AspnetIdentity");
	}
}

public class ConfigureIdentityUserLogin : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
	public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> b)
	{
		b.Property<string>("LoginProvider")
			.HasColumnType("text");

		b.Property<string>("ProviderKey")
			.HasColumnType("text");

		b.Property<string>("ProviderDisplayName")
			.HasColumnType("text");

		b.Property<Guid>("UserId")
			.IsRequired()
			.HasColumnType("uuid");

		b.HasKey("LoginProvider", "ProviderKey");

		b.HasIndex("UserId");

		b.ToTable("AspNetUserLogins", "AspnetIdentity");

		b.HasOne<Models.Account>()
			.WithMany()
			.HasForeignKey("UserId")
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();
	}
}

public class ConfigureIdentityUserRole : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
	public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> b)
	{
		b.Property<Guid>("UserId")
			.HasColumnType("uuid");

		b.Property<Guid>("RoleId")
			.HasColumnType("uuid");

		b.HasKey("UserId", "RoleId");

		b.HasIndex("RoleId");

		b.ToTable("AspNetUserRoles", "AspnetIdentity");

		b.HasOne<IdentityRole<Guid>>()
			.WithMany()
			.HasForeignKey("RoleId")
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		b.HasOne<Models.Account>()
			.WithMany()
			.HasForeignKey("UserId")
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();
	}
}

public class ConfigureIdentityUserToken : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
	public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> b)
	{
		b.Property<Guid>("UserId")
			.HasColumnType("uuid");

		b.Property<string>("LoginProvider")
			.HasColumnType("text");

		b.Property<string>("Name")
			.HasColumnType("text");

		b.Property<string>("Value")
			.HasColumnType("text");

		b.HasKey("UserId", "LoginProvider", "Name");

		b.ToTable("AspNetUserTokens", "AspnetIdentity");

		b.HasOne<Models.Account>()
			.WithMany()
			.HasForeignKey("UserId")
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();
	}
}