// Modern JavaScript functionality for Asreyion.Server
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(function() {
    'use strict';

    // Wait for DOM to be fully loaded
    document.addEventListener('DOMContentLoaded', function() {
        // Immediately hide nested dropdowns before Bootstrap initializes
        const allNestedMenus = document.querySelectorAll('.nested-dropdown-menu');
        allNestedMenus.forEach(menu => {
            menu.style.display = 'none';
        });
        
        initializeNavbar();
        initializeSmoothScroll();
        initializeAnimations();
        initializeFormEnhancements();
        initializeMobileMenu();
        initializeDesktopDropdowns();
    });

    /**
     * Initialize navbar scroll effects
     */
    function initializeNavbar() {
        const navbar = document.querySelector('.navbar');
        if (!navbar) return;

        let lastScroll = 0;

        window.addEventListener('scroll', function() {
            const currentScroll = window.pageYOffset;
            
            // Add shadow on scroll
            if (currentScroll > 10) {
                navbar.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.4)';
            } else {
                navbar.style.boxShadow = '0 2px 8px rgba(0, 0, 0, 0.3)';
            }

            lastScroll = currentScroll;
        });
    }

    /**
     * Initialize smooth scrolling for anchor links
     */
    function initializeSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function(e) {
                const href = this.getAttribute('href');
                if (href === '#' || !href) return;

                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    const headerOffset = 80;
                    const elementPosition = target.getBoundingClientRect().top;
                    const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

                    window.scrollTo({
                        top: offsetPosition,
                        behavior: 'smooth'
                    });
                }
            });
        });
    }

    /**
     * Initialize scroll-triggered animations
     */
    function initializeAnimations() {
        const observerOptions = {
            root: null,
            rootMargin: '0px',
            threshold: 0.1
        };

        const observer = new IntersectionObserver(function(entries) {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-slide-up');
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        // Observe elements with animation classes
        document.querySelectorAll('.card, .hero, section').forEach(el => {
            observer.observe(el);
        });
    }

    /**
     * Initialize form enhancements
     */
    function initializeFormEnhancements() {
        // Add floating label effects
        const formInputs = document.querySelectorAll('.form-control, .form-select');
        formInputs.forEach(input => {
            input.addEventListener('focus', function() {
                this.parentElement.classList.add('focused');
            });

            input.addEventListener('blur', function() {
                if (!this.value) {
                    this.parentElement.classList.remove('focused');
                }
            });

            // Check initial state
            if (input.value) {
                input.parentElement.classList.add('focused');
            }
        });

        // Add form validation styling
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            form.addEventListener('submit', function(e) {
                const inputs = form.querySelectorAll('input, select, textarea');
                let isValid = true;

                inputs.forEach(input => {
                    if (input.hasAttribute('required') && !input.value) {
                        isValid = false;
                        input.classList.add('is-invalid');
                    } else {
                        input.classList.remove('is-invalid');
                    }
                });

                if (!isValid && form.getAttribute('novalidate') !== null) {
                    e.preventDefault();
                }
            });
        });
    }

    /**
     * Initialize mobile menu enhancements
     */
    function initializeMobileMenu() {
        const navbarToggler = document.querySelector('.navbar-toggler');
        const navbarCollapse = document.querySelector('.navbar-collapse');

        if (!navbarToggler || !navbarCollapse) return;

        // Close menu when clicking outside
        document.addEventListener('click', function(e) {
            if (!navbarCollapse.contains(e.target) && !navbarToggler.contains(e.target)) {
                if (navbarCollapse.classList.contains('show')) {
                    navbarToggler.click();
                }
            }
        });

        // Close menu when clicking a link (except dropdown toggles)
        const navLinks = navbarCollapse.querySelectorAll('.nav-link:not(.dropdown-toggle)');
        navLinks.forEach(link => {
            link.addEventListener('click', function() {
                if (navbarCollapse.classList.contains('show')) {
                    navbarToggler.click();
                }
            });
        });

        // Handle top-level dropdowns on mobile
        const dropdownToggles = navbarCollapse.querySelectorAll('.nav-item.dropdown > .dropdown-toggle');
        dropdownToggles.forEach(toggle => {
            toggle.addEventListener('click', function(e) {
                if (window.innerWidth < 992) {
                    e.preventDefault();
                    e.stopPropagation();
                    const dropdown = this.parentElement;
                    const dropdownMenu = dropdown.querySelector('.dropdown-menu');
                    
                    // Toggle current dropdown
                    if (dropdownMenu.style.display === 'block') {
                        dropdownMenu.style.display = '';
                    } else {
                        // Close other dropdowns first
                        document.querySelectorAll('.nav-item.dropdown .dropdown-menu').forEach(menu => {
                            if (menu !== dropdownMenu) {
                                menu.style.display = '';
                            }
                        });
                        dropdownMenu.style.display = 'block';
                    }
                }
            });
        });

        // Handle nested dropdowns on mobile
        const dropdownSubmenus = document.querySelectorAll('.dropdown-submenu');
        dropdownSubmenus.forEach(submenu => {
            const toggle = submenu.querySelector('.dropdown-toggle');
            if (toggle) {
                toggle.addEventListener('click', function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    submenu.classList.toggle('show');
                });
            }
        });

        // Handle dropdown item clicks on mobile
        const dropdownItems = navbarCollapse.querySelectorAll('.dropdown-item:not(.dropdown-toggle)');
        dropdownItems.forEach(item => {
            item.addEventListener('click', function(e) {
                if (window.innerWidth < 992 && !this.classList.contains('dropdown-toggle')) {
                    // Close the mobile menu after clicking a dropdown item
                    if (navbarCollapse.classList.contains('show')) {
                        setTimeout(() => {
                            navbarToggler.click();
                        }, 100);
                    }
                }
            });
        });
    }

    /**
     * Initialize desktop dropdown behavior
     */
    function initializeDesktopDropdowns() {
        // Hide all nested dropdowns on page load
        const allNestedMenus = document.querySelectorAll('.nested-dropdown-menu');
        allNestedMenus.forEach(menu => {
            menu.style.display = 'none';
        });
        
        // Store timeout references for clearing
        const hideTimeouts = new Map();
        
        // Handle nested dropdowns on desktop - only show when hovering the specific item
        const dropdownSubmenus = document.querySelectorAll('.dropdown-submenu');
        
        dropdownSubmenus.forEach(submenu => {
            const submenuItem = submenu.querySelector('.dropdown-item');
            const submenuMenu = submenu.querySelector('.nested-dropdown-menu');
            
            if (submenuItem && submenuMenu) {
                // Ensure it's hidden initially
                submenuMenu.style.display = 'none';
                
                // Show nested menu when hovering the specific item
                submenuItem.addEventListener('mouseenter', function() {
                    if (window.innerWidth >= 992) {
                        // Clear any pending hide timeout
                        if (hideTimeouts.has(submenuMenu)) {
                            clearTimeout(hideTimeouts.get(submenuMenu));
                            hideTimeouts.delete(submenuMenu);
                        }
                        submenuMenu.style.display = 'block';
                        // Add hover class to keep parent highlighted
                        submenuItem.classList.add('dropdown-item-active');
                    }
                });
                
                // Hide nested menu when leaving the specific item (with delay)
                submenuItem.addEventListener('mouseleave', function() {
                    if (window.innerWidth >= 992) {
                        // Add delay to allow moving to nested menu
                        const timeout = setTimeout(() => {
                            submenuMenu.style.display = 'none';
                            submenuItem.classList.remove('dropdown-item-active');
                            hideTimeouts.delete(submenuMenu);
                        }, 200);
                        hideTimeouts.set(submenuMenu, timeout);
                    }
                });
                
                // Keep nested menu open when hovering over it
                submenuMenu.addEventListener('mouseenter', function() {
                    if (window.innerWidth >= 992) {
                        // Clear any pending hide timeout
                        if (hideTimeouts.has(submenuMenu)) {
                            clearTimeout(hideTimeouts.get(submenuMenu));
                            hideTimeouts.delete(submenuMenu);
                        }
                        submenuMenu.style.display = 'block';
                        // Keep parent highlighted
                        submenuItem.classList.add('dropdown-item-active');
                    }
                });
                
                // Hide nested menu when leaving it
                submenuMenu.addEventListener('mouseleave', function() {
                    if (window.innerWidth >= 992) {
                        submenuMenu.style.display = 'none';
                        submenuItem.classList.remove('dropdown-item-active');
                    }
                });
            }
        });
        
        // Handle parent dropdown hover states
        const parentDropdownItems = document.querySelectorAll('.nav-item.dropdown');
        parentDropdownItems.forEach(dropdown => {
            const dropdownToggle = dropdown.querySelector('.dropdown-toggle');
            const dropdownMenu = dropdown.querySelector('.dropdown-menu');
            
            if (dropdownToggle && dropdownMenu) {
                // Add hover class when parent dropdown is open
                dropdownToggle.addEventListener('mouseenter', function() {
                    if (window.innerWidth >= 992) {
                        dropdownToggle.classList.add('nav-link-active');
                    }
                });
                
                dropdownMenu.addEventListener('mouseenter', function() {
                    if (window.innerWidth >= 992) {
                        dropdownToggle.classList.add('nav-link-active');
                    }
                });
                
                // Remove hover class when leaving
                dropdownToggle.addEventListener('mouseleave', function() {
                    if (window.innerWidth >= 992) {
                        dropdownToggle.classList.remove('nav-link-active');
                    }
                });
                
                dropdownMenu.addEventListener('mouseleave', function() {
                    if (window.innerWidth >= 992) {
                        dropdownToggle.classList.remove('nav-link-active');
                    }
                });
            }
        });
        
        // Hide all nested dropdowns when leaving parent dropdown (with delay)
        const parentDropdowns = document.querySelectorAll('.dropdown-menu');
        parentDropdowns.forEach(dropdown => {
            dropdown.addEventListener('mouseleave', function() {
                const nestedMenus = dropdown.querySelectorAll('.nested-dropdown-menu');
                nestedMenus.forEach(menu => {
                    // Add delay to allow moving to nested menu
                    setTimeout(() => {
                        menu.style.display = 'none';
                    }, 200);
                });
            });
        });
    }

    /**
     * Utility function to debounce function calls
     */
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    /**
     * Utility function to throttle function calls
     */
    function throttle(func, limit) {
        let inThrottle;
        return function(...args) {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    /**
     * Show toast notification
     */
    function showToast(message, type = 'info') {
        // Remove existing toasts
        const existingToasts = document.querySelectorAll('.toast-notification');
        existingToasts.forEach(toast => toast.remove());

        const toast = document.createElement('div');
        toast.className = `toast-notification alert alert-${type}`;
        toast.style.cssText = `
            position: fixed;
            top: 100px;
            right: 20px;
            z-index: 9999;
            min-width: 300px;
            animation: slideInRight 0.3s ease-out;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
        `;
        toast.textContent = message;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    /**
     * Add CSS animations for toasts
     */
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        @keyframes slideOutRight {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(100%);
                opacity: 0;
            }
        }
    `;
    document.head.appendChild(style);

    // Expose utility functions globally
    window.AsreyionUtils = {
        debounce,
        throttle,
        showToast
    };

})();
