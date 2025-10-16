window.dashboard = {
    initialize: async function () {
        console.log("Dashboard initialized");
        // Add any initialization logic here
        
        // Initialize Font Awesome if you're using it
        if (typeof FontAwesome !== 'undefined') {
            FontAwesome.dom.watch();
        }

        // Add smooth fade-in animation
        document.querySelector('.dashboard-page')?.classList.add('visible');
    },

    showNotification: function (message, type = 'info') {
        const colors = {
            success: '#10B981',
            error: '#EF4444',
            warning: '#F59E0B',
            info: '#3B82F6'
        };

        // Create notification container if it doesn't exist
        let container = document.getElementById('notification-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'notification-container';
            container.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
            `;
            document.body.appendChild(container);
        }

        // Create notification element
        const notification = document.createElement('div');
        notification.style.cssText = `
            background-color: ${colors[type] || colors.info};
            color: white;
            padding: 12px 24px;
            border-radius: 6px;
            margin-bottom: 10px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.2);
            animation: slideIn 0.3s ease-out;
            display: flex;
            align-items: center;
            gap: 8px;
        `;

        notification.innerHTML = `
            <span>${message}</span>
            <button style="background: none; border: none; color: white; cursor: pointer; padding: 0; margin-left: 10px;"
                    onclick="this.parentElement.remove()">×</button>
        `;

        container.appendChild(notification);

        // Remove notification after 5 seconds
        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease-out';
            setTimeout(() => notification.remove(), 300);
        }, 5000);
    }
};

// Add required CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from { transform: translateX(100%); opacity: 0; }
        to { transform: translateX(0); opacity: 1; }
    }

    @keyframes slideOut {
        from { transform: translateX(0); opacity: 1; }
        to { transform: translateX(100%); opacity: 0; }
    }

    .dashboard-page {
        opacity: 0;
        transition: opacity 0.3s ease-in-out;
    }

    .dashboard-page.visible {
        opacity: 1;
    }
`;
document.head.appendChild(style);