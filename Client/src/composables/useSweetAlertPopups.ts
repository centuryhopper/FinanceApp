import Swal from "sweetalert2";


export default function useSweetAlertPopups()
{
    const showFeedbackPopup = async (
        success: boolean = true,
        successMsg: string,
        failMsg: string,
    ): Promise<void> =>
    {
        const result = await Swal.fire({
            title: success ? "Success!" : "Failure...",
            text: success
                ? successMsg
                : failMsg,
            icon: success ? "success" : "error",
            confirmButtonText: "OK",
            customClass: {
                popup: "swal-dark",
            },
            allowOutsideClick: false,
        });
    };

    const confirmationPopup = async (
        title: string = "Are you sure?",
        text: string = "Do you want to proceed?",
        confirmedCb: () => Promise<void>,
        dismissedCb: () => Promise<void>,
    ): Promise<void> =>
    {
        const result = await Swal.fire({
            title,
            text,
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Yes",
            cancelButtonText: "No",
            confirmButtonColor: '#28a745',
            cancelButtonColor: 'red',
            customClass: {
                popup: "swal-dark",
            },
            allowOutsideClick: false,
        });

        if (result.isConfirmed)
        {
            confirmedCb()
        }
        else if (result.isDismissed)
        {
            dismissedCb()
        }
    };

    return {
        showFeedbackPopup,
        confirmationPopup,
    }
}