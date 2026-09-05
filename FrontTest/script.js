

const statusElement = document.getElementById("status");

const usernameInput = document.getElementById("username");

const messageInput = document.getElementById("message");

const groupNameInput = document.getElementById("groupName");

const sendAllButton = document.getElementById("sendAll");

const joinGroupButton = document.getElementById("joinGroup");

const sendGroupButton = document.getElementById("sendGroup");

const messagesContainer = document.getElementById("messages");



// Define Connection

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7284/message")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();




// Start Connection

async function startConnection() {

    try {

        await connection.start();

        console.log("Connected!");

        statusElement.textContent = "Connected";

        statusElement.classList.remove("disconnected");

        statusElement.classList.add("connected");

    }
    catch (error) {

        console.error(
            "Connection failed:",
            error
        );

        statusElement.textContent = "Disconnected";

        statusElement.classList.remove("connected");

        statusElement.classList.add("disconnected");

    }
}


// Call back function
// Receive Message

connection.on(
    "ReceiveMessage",
    function (user, message) {

        console.log(
            "Received:",
            user,
            message
        );

        addMessage(
            user,
            message
        );
    }
);


// ========================================
// Add Message To UI
// ========================================

function addMessage(user, message) {

    const messageElement =
        document.createElement("div");

    messageElement.classList.add("message");

    messageElement.innerHTML = `
        <strong>${user}:</strong>
        ${message}
    `;

    messagesContainer.appendChild(
        messageElement
    );

    messagesContainer.scrollTop =
        messagesContainer.scrollHeight;
}


// ========================================
// Send Message To Everyone
// ========================================

sendAllButton.addEventListener(
    "click",
    async function () {

        const username =
            usernameInput.value;

        const message =
            messageInput.value;


        if (!username || !message) {

            alert(
                "Enter username and message"
            );

            return;
        }


        try {

            await connection.invoke(
                "SendToAll",
                username,
                message
            );

            messageInput.value = "";

        }
        catch (error) {

            console.error(
                "SendToAll error:",
                error
            );

        }

    }
);


// ========================================
// Join Group
// ========================================

joinGroupButton.addEventListener(
    "click",
    async function () {

        const groupName =
            groupNameInput.value;


        if (!groupName) {

            alert(
                "Enter group name"
            );

            return;
        }


        try {

            await connection.invoke(
                "JoinGroup",
                groupName
            );

            console.log(
                "Joined group:",
                groupName
            );

            alert(
                `Joined ${groupName}`
            );

        }
        catch (error) {

            console.error(
                "Join group error:",
                error
            );

        }

    }
);


// ========================================
// Send Message To Group
// ========================================

sendGroupButton.addEventListener(
    "click",
    async function () {

        const username =
            usernameInput.value;

        const message =
            messageInput.value;

        const groupName =
            groupNameInput.value;


        if (
            !username ||
            !message ||
            !groupName
        ) {

            alert(
                "Enter username, message and group"
            );

            return;
        }


        try {

            await connection.invoke(
                "SendToGroup",
                groupName,
                username,
                message
            );

            messageInput.value = "";

        }
        catch (error) {

            console.error(
                "SendToGroup error:",
                error
            );

        }

    }
);


// ========================================
// Connection Events
// ========================================

connection.onreconnecting(function () {

    console.log(
        "Reconnecting..."
    );

    statusElement.textContent =
        "Reconnecting...";

});


connection.onreconnected(function () {

    console.log(
        "Reconnected!"
    );

    statusElement.textContent =
        "Connected";

});


connection.onclose(function () {

    console.log(
        "Connection closed"
    );

    statusElement.textContent =
        "Disconnected";

});



// Start

startConnection();


connection.on("ConnectionTest", function (connectionId) 
 {
    console.log("ConnectionTest received. Connection ID:", connectionId);
 });