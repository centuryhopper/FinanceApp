
PLAID_FUNCTIONS = {
    openPlaid: function (linkToken, dotnetRef) {
        // Create Plaid Link handler
        const handler = window.Plaid.create({
            token: linkToken,

            onSuccess: function (public_token, metadata) {
                dotnetRef.invokeMethodAsync("PlaidSuccess", public_token);
            },

            onExit: function (err, metadata) {
                if (err) {
                    console.error("Plaid exited with error:", err);
                }

                dotnetRef.invokeMethodAsync("PlaidExit");
            }
        });

        handler.open();
    }

}
