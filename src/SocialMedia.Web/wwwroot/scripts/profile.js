const InitialiseFollowButtons = () => {
	const followButton = document.getElementById("follow");
	if (followButton) {
		followButton.addEventListener("click", (e) => {
			e.preventDefault();
			document.getElementById("followForm").submit();
		});
		return;
	}

	const unfollowButton = document.getElementById("unfollow");
	if (unfollowButton) {
		unfollowButton.addEventListener("click", (e) => {
			e.preventDefault();
			document.getElementById("unfollowForm").submit();
		});
		return;
	}
};

const InitialiseProfile = () => {
	if (!document.getElementById("profile")) {
		return;
	}

	InitialiseFollowButtons();
};

export default InitialiseProfile;
