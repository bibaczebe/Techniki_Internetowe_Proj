// ============================================================================
//  Tymczasowy magazyn ASP.NET Core Identity dzialajacy w pamieci (bez bazy).
//  Pozwala uruchomic logowanie / rejestracje, zanim powstanie warstwa EF Core.
//
//  PO DODANIU BAZY: w Program.cs podmieniacie
//      .AddUserStore<TemporaryUserStore>().AddRoleStore<TemporaryRoleStore>()
//  na
//      .AddEntityFrameworkStores<NazwaWaszegoKontekstu>()
//  i kasujecie ten plik.
//
//  Dane sa trzymane w polach STATIC, dzieki czemu przezywaja miedzy zadaniami
//  (magazyn jest rejestrowany jako scoped, czyli tworzony na nowo przy kazdym
//  requescie). Po restarcie aplikacji dane znikaja, to normalne dla makiety.
//
//  Rozbicie na dwie klasy jest celowe: IUserStore i IRoleStore maja metody
//  FindByIdAsync / FindByNameAsync o identycznej sygnaturze a roznym typie
//  zwracanym, wiec w jednej klasie sie nie kompiluja.
// ============================================================================
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Techniki_Internetowe_Proj.Services;

// Wspolne, statyczne miejsce na dane (uzywane przez oba magazyny).
internal static class TemporaryStore
{
    public static readonly List<IdentityUser> Users = new();
    public static readonly List<IdentityRole> Roles = new();

    // userId -> zbior ZNORMALIZOWANYCH nazw rol
    public static readonly Dictionary<string, HashSet<string>> UserRoles = new();

    public static readonly object Gate = new();
}

public class TemporaryUserStore :
    IUserStore<IdentityUser>,
    IUserPasswordStore<IdentityUser>,
    IUserEmailStore<IdentityUser>,
    IUserRoleStore<IdentityUser>,
    IUserLockoutStore<IdentityUser>,
    IUserSecurityStampStore<IdentityUser>
{
    public void Dispose() { }

    // ---------- IUserStore ----------
    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.Id);

    public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.UserName);

    public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.NormalizedUserName);

    public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            TemporaryStore.Users.Add(user);
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            var index = TemporaryStore.Users.FindIndex(u => u.Id == user.Id);
            if (index >= 0)
            {
                TemporaryStore.Users[index] = user;
            }
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            TemporaryStore.Users.RemoveAll(u => u.Id == user.Id);
            TemporaryStore.UserRoles.Remove(user.Id);
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            return Task.FromResult(TemporaryStore.Users.FirstOrDefault(u => u.Id == userId));
        }
    }

    public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            return Task.FromResult(TemporaryStore.Users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName));
        }
    }

    // ---------- IUserPasswordStore ----------
    public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.PasswordHash);

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

    // ---------- IUserEmailStore ----------
    public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.Email);

    public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.EmailConfirmed);

    public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            return Task.FromResult(TemporaryStore.Users.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail));
        }
    }

    public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.NormalizedEmail);

    public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    // ---------- IUserRoleStore ----------
    // Uwaga: UserManager przekazuje tu nazwy rol JUZ ZNORMALIZOWANE (wielkimi literami).
    public Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            if (!TemporaryStore.UserRoles.TryGetValue(user.Id, out var set))
            {
                set = new HashSet<string>();
                TemporaryStore.UserRoles[user.Id] = set;
            }
            set.Add(roleName);
        }
        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            if (TemporaryStore.UserRoles.TryGetValue(user.Id, out var set))
            {
                set.Remove(roleName);
            }
        }
        return Task.CompletedTask;
    }

    public Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            IList<string> roles = new List<string>();
            if (TemporaryStore.UserRoles.TryGetValue(user.Id, out var normalizedNames))
            {
                foreach (var normalized in normalizedNames)
                {
                    var role = TemporaryStore.Roles.FirstOrDefault(r => r.NormalizedName == normalized);
                    roles.Add(role != null ? role.Name : normalized);
                }
            }
            return Task.FromResult(roles);
        }
    }

    public Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            var inRole = TemporaryStore.UserRoles.TryGetValue(user.Id, out var set) && set.Contains(roleName);
            return Task.FromResult(inRole);
        }
    }

    public Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            IList<IdentityUser> result = TemporaryStore.Users
                .Where(u => TemporaryStore.UserRoles.TryGetValue(u.Id, out var set) && set.Contains(roleName))
                .ToList();
            return Task.FromResult(result);
        }
    }

    // ---------- IUserLockoutStore ----------
    public Task<DateTimeOffset?> GetLockoutEndDateAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.LockoutEnd);

    public Task SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    public Task<int> IncrementAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task ResetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public Task<int> GetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.AccessFailedCount);

    public Task<bool> GetLockoutEnabledAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.LockoutEnabled);

    public Task SetLockoutEnabledAsync(IdentityUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    // ---------- IUserSecurityStampStore ----------
    public Task SetSecurityStampAsync(IdentityUser user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task<string> GetSecurityStampAsync(IdentityUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.SecurityStamp);
}

public class TemporaryRoleStore : IRoleStore<IdentityRole>
{
    public void Dispose() { }

    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            TemporaryStore.Roles.Add(role);
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            var index = TemporaryStore.Roles.FindIndex(r => r.Id == role.Id);
            if (index >= 0)
            {
                TemporaryStore.Roles[index] = role;
            }
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            TemporaryStore.Roles.RemoveAll(r => r.Id == role.Id);
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        => Task.FromResult(role.Id);

    public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        => Task.FromResult(role.Name);

    public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        => Task.FromResult(role.NormalizedName);

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            return Task.FromResult(TemporaryStore.Roles.FirstOrDefault(r => r.Id == roleId));
        }
    }

    public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        lock (TemporaryStore.Gate)
        {
            return Task.FromResult(TemporaryStore.Roles.FirstOrDefault(r => r.NormalizedName == normalizedRoleName));
        }
    }
}
