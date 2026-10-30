# Blog URL routing

Category URLs include the complete parent category slug path, for example `/blog/category/gaming/runescape`.

Post URLs include the selected category path followed by the post slug, for example `/blog/posts/gaming/runescape/my-post`. When a post is assigned to multiple categories, the URL service consistently selects the alphabetically first full category path. Uncategorised posts use `/blog/posts/my-post`.

The previous `/blog/post/my-post` route permanently redirects to the new canonical URL. Existing leaf-only category URLs continue to resolve.
