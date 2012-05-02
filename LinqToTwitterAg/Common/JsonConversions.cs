//using System;
//using System.Globalization;

//namespace LinqToTwitter
//{
//    internal static class JsonConversions
//    {
//        /// <summary>
//        /// Maps a json user into a new User object
//        /// </summary>
//        /// <param name="user">json user</param>
//        /// <returns>User</returns>
//        internal static User ToUser(this Json.User user)
//        {
//            user = user ?? new Json.User(); // null object handling...
//            var status = user.status != null ? user.status.ToStatus() : null;
//            var userId = user.id.ToString(CultureInfo.InvariantCulture);

//            // let's logically group this stuff...
//            return new User
//            {
//                Identifier = new UserIdentifier
//                {
//                    ID = userId,
//                    UserID = userId,
//                    ScreenName = user.screen_name
//                },
//                Name = user.name,
//                Location = user.location,
//                Description = user.description,
//                Url = user.url,

//                ProfileImageUrl = user.profile_image_url,
//                ProfileImageUrlHttps = user.profile_image_url_https,
//                CreatedAt = user.created_at,

//                ProfileBackgroundColor = user.profile_background_color,
//                ProfileBackgroundImageUrl = user.profile_background_image_url,
//                ProfileBackgroundImageUrlHttps = user.profile_background_image_url,
//                ProfileBackgroundTile = user.profile_background_tile,
//                ProfileLinkColor = user.profile_link_color,
//                ProfileTextColor = user.profile_text_color,
//                ProfileSidebarBorderColor = user.profile_sidebar_border_color,
//                ProfileSidebarFillColor = user.profile_sidebar_fill_color,
//                ProfileUseBackgroundImage = user.profile_use_background_image,

//                DefaultProfileImage = user.default_profile_image,
//                DefaultProfile = user.default_profile,

//                TimeZone = user.time_zone,
//                Lang = user.lang,
//                GeoEnabled = user.geo_enabled,
//                ShowAllInlineMedia = user.show_all_inline_media,
//                UtcOffset = user.utc_offset,
//                IsTranslator = user.is_translator,
//                Protected = user.is_protected,
//                Verified = user.verified,
//                Notifications = user.notifications,
//                ContributorsEnabled = user.contributors_enabled,

//                Following = user.following,
//                FollowRequestSent = user.follow_request_sent,

//                FriendsCount = user.friends_count,
//                StatusesCount = user.statuses_count,
//                FavoritesCount = user.favourites_count,
//                FollowersCount = user.followers_count,
//                ListedCount = user.listed_count,

//                Status = status,
//                // CursorMovement = Cursors.CreateCursors(GrandParentOrNull(user))
//            };
//        }

//        /// <summary>
//        /// Maps a json status into a new Status object
//        /// </summary>
//        /// <param name="status">json Status</param>
//        /// <returns>Status</returns>
//        internal static Status ToStatus(this Json.Status status)
//        {
//            status = status ?? new Json.Status(); // null object handling...
//            var user = status.user != null ? status.user.ToUser() : null;
//            // TODO: decide between Place and Location
//            var place = status.place != null ? status.place.ToPlace() : null;
//            var geo = default(Geo); // TODO status.Geo
//            var coordinates = default(Coordinate); // TODO status.coordinates

//            return new Status
//            {
//                StatusID = status.id.ToString(CultureInfo.InvariantCulture),
//                Truncated = status.truncated,
//                Favorited = status.favorited,
//                Retweeted = status.retweeted,
//                InReplyToStatusID = status.in_reply_to_status_id.ToString(CultureInfo.InvariantCulture),
//                InReplyToUserID = status.in_reply_to_user_id.ToString(CultureInfo.InvariantCulture),
//                InReplyToScreenName = status.in_reply_to_screen_name,
//                Geo = geo,
//                User = user,
//                CreatedAt = status.created_at,
//                RetweetCount = status.retweet_count,
//                Text = status.text,
//                Source = status.source,
//                Coordinates = coordinates,
//                Place = place 
//            };
//        }

//        /// <summary>
//        /// Maps a json place into a new Place object
//        /// </summary>
//        /// <param name="place">json place</param>
//        /// <returns>Place</returns>
//        internal static Place ToPlace(this Json.Place place)
//        {
//            place = place ?? new Json.Place(); // null object handling...
//            var pt = place.placeType ?? new Json.PlaceType { code = 0, name = string.Empty };

//            return new Place
//            {
//                Country = place.country ?? string.Empty,
//                CountryCode = place.countryCode ?? string.Empty,
//                Name = place.name ?? string.Empty,
//                Url = place.url ?? string.Empty,
//                BoundingBox = null,
//                ContainedWithin = null,
//                FullName = null,
//                Geometry = null,
//                PolyLines = null,
//                PlaceType = pt.name,
//                ID = null
//            };
//        }

//        /// <summary>
//        /// Maps a json place into a new Location object
//        /// </summary>
//        /// <param name="place">json place</param>
//        /// <returns>Location</returns>
//        internal static Location ToLocation(this Json.Place place)
//        {
//            place = place ?? new Json.Place(); // null object handling...
//            var pt = place.placeType ?? new Json.PlaceType { code = 0, name = string.Empty };

//            return new Location
//            {
//                Country = place.country ?? string.Empty,
//                CountryCode = place.countryCode ?? string.Empty,
//                Name = place.name ?? string.Empty,
//                ParentID = place.parentid.ToString(CultureInfo.InvariantCulture),
//                PlaceTypeName = pt.name,
//                PlaceTypeNameCode = pt.code,
//                Url = place.url ?? string.Empty,
//                WoeID = place.woeid.ToString(CultureInfo.InvariantCulture)
//            };
//        }

//        /// <summary>
//        /// Maps a json sleep_time into a new SleepTime object
//        /// </summary>
//        /// <param name="sleep_time">json sleep_time</param>
//        /// <returns>SleepTime</returns>
//        internal static SleepTime ToSleepTime(this Json.SleepTime sleep_time)
//        {
//            sleep_time = sleep_time ?? new Json.SleepTime();    // null object handling

//            return new SleepTime
//            {
//                StartHour = sleep_time.start_time,
//                EndHour = sleep_time.end_time,
//                Enabled = sleep_time.enabled
//            };
//        }

//        /// <summary>
//        /// Maps a json time_zone into a new TZInfo object
//        /// </summary>
//        /// <param name="time_zone">json TimeZone object</param>
//        /// <returns>TZInfo</returns>
//        internal static TZInfo ToTZInfo(this Json.TimeZone time_zone)
//        {
//            time_zone = time_zone ?? new Json.TimeZone();   // null object handling

//            return new TZInfo
//            {
//                Name = time_zone.name ?? String.Empty,
//                TzInfoName = time_zone.tzinfo_name ?? String.Empty,
//                UtcOffset = time_zone.utc_offset
//            };
//        }
//    }
//}
