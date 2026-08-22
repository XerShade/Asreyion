---
title: "Fixing WordPress Media Library: Updating Upload Dates for Gallery Projects"
date: 2023-11-26
categories: 
  - "coding"
tags: 
  - "dates"
  - "media"
  - "plugin"
  - "sync"
  - "wordpress"
---

Hey everyone,

While working on the gallery section of the website for my gaming screenshots and other images, I ran into a small issue with how WordPress handles file uploads. Specifically, when you upload a file through the media library, WordPress sets the `post_date` and `post_modified` fields in the database to the date the file was uploaded.

This becomes a problem when you need to filter older screenshots from several years ago — say, anywhere from three years to over a decade ago. The issue is that filtering by upload date in the database won’t help, since it only reflects the upload time, not the actual creation or modification date of the file. And scanning the files directly to check creation or modified dates is both inefficient and resource-intensive. This is exactly why we store this kind of metadata in the database in the first place.

### The Solution

To fix this, I created a simple one-file plugin that adds a button to the WordPress media library. This button scans the media files in the database and updates their `post_date` and `post_modified` fields to reflect the actual last modification date of the files, rather than the upload date.

Now that this issue is resolved, I can get back to working on the gallery sections for my gaming screenshots and other images. Those should be up and live in a few days, assuming everything goes according to plan. I’ll make another post once they’re live to let you know.

### The Code

If you’ve encountered a similar issue or need this functionality for your own website, feel free to use the code I wrote. It’s not the most optimized solution, but it works for my needs and may be helpful for yours as well.

You can find the most up to date code [here on GitHub](https://gist.github.com/XerShade/37cb73ecad6b294571739c2c513424e2).

```php
<?php
/*
Plugin Name: Sync Media Dates
Description: Adds a button under the Media Library tab to sync the post date and post date GMT of media files to be the date the file was last modified.
Version: 1.0
Author: XerShade
*/

function sync_media_dates_page() {
    add_media_page(
        'Sync Media Dates',
        'Sync Media Dates',
        'manage_options',
        'sync-media-dates',
        'sync_media_dates_callback'
    );
}
add_action('admin_menu', 'sync_media_dates_page');

function sync_media_dates_callback() {
    if (isset($_POST['sync_media_dates_nonce']) && wp_verify_nonce($_POST['sync_media_dates_nonce'], 'sync_media_dates_action')) {
        // Run the code to sync media dates.
        sync_media_dates();
        echo '<div class="updated"><p>Media dates synced successfully!</p></div>';
    }

    ?>
    <div class="wrap">
        <h1>Sync Media Dates</h1>
        <form method="post">
            <?php wp_nonce_field('sync_media_dates_action', 'sync_media_dates_nonce'); ?>
            <p>This will sync the post date and post date GMT of all media files to be the date the file was last modified.</p>
            <p><button type="submit" class="button button-primary">Sync Media Dates</button></p>
        </form>
    </div>
    <?php
}

function sync_media_dates() {
    $media_query = new WP_Query(
        array(
            'post_type'      => 'attachment',
            'posts_per_page' => -1,
            'post_status'    => 'any',
        )
    );

    if ($media_query->have_posts()) {
        while ($media_query->have_posts()) {
            $media_query->the_post();

            $attachment_id = get_the_ID();
            $file_path     = get_attached_file($attachment_id);
            $file_created  = filectime($file_path);
            $file_modified  = filemtime($file_path);

            // Sync both post_date and post_date_gmt to the file creation date.
            wp_update_post(
                array(
                    'ID'                => $attachment_id,
                    'post_date'         => date('Y-m-d H:i:s', $file_modified),
                    'post_date_gmt'     => get_gmt_from_date(date('Y-m-d H:i:s', $file_modified)),
                    'post_modified'     => date('Y-m-d H:i:s', $file_modified),
                    'post_modified_gmt' => get_gmt_from_date(date('Y-m-d H:i:s', $file_modified)),
                )
            );
        }

        wp_reset_postdata();
    }
}
```

Please note that I’m providing the code as-is, and I won’t be offering support to expand or modify it beyond its current functionality. It was made to solve a very specific problem on my site. However, if you make any improvements or changes, feel free to post them on GitHub and link them to me. I’d be happy to review and potentially update the code.
