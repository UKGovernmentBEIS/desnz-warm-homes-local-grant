const ConfirmationPage = (function (document) {
    
    const oldValues = {};
    
    // Whilst the primary and replica controls have the same value any changes to the primary is also made to the
    // replica.
    function duplicateDefaultValue(primaryId, replicaId) {
        const primaryControl = document.getElementById(primaryId);
        const replicaControl = document.getElementById(replicaId);
        
        oldValues[primaryId] = primaryControl.value;
        
        primaryControl.addEventListener("input", (event) => {
            if (oldValues[primaryId] === replicaControl.value) {
                replicaControl.value = primaryControl.value;
            }
            oldValues[primaryId] = primaryControl.value;
        })
    }
    
    return { duplicateDefaultValue };
})(document);