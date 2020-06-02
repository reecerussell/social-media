const SetReplyToId = (e) => {
	e.preventDefault();
	const commentId = e.target.getAttribute("data-comment-id");
	if (!commentId) {
		return;
	}

	const replyToField = document.getElementById("ReplyToId");
	replyToField.value = commentId;

	document.getElementById("Comment").focus();
};

const DeleteComment = (e) => {
	e.preventDefault();
	const commentId = e.target.getAttribute("data-comment-id");
	if (!commentId) {
		return;
	}

	const commentIdField = document.getElementById("deleteCommentId");
	commentIdField.value = commentId;

	document.getElementById("deleteComment").submit();
};

const InitialiseLikeButtons = () => {
	const likeButton = document.getElementById("like");
	if (likeButton) {
		likeButton.addEventListener("click", (e) => {
			e.preventDefault();
			document.getElementById("likeForm").submit();
		});
		return;
	}

	const unlikeButton = document.getElementById("unlike");
	if (unlikeButton) {
		unlikeButton.addEventListener("click", (e) => {
			e.preventDefault();
			document.getElementById("unlikeForm").submit();
		});
		return;
	}
};

const InitialisePost = () => {
	if (!document.getElementById("post")) {
		return;
	}

	const replyButtons = document.querySelectorAll(".js-reply-to");
	for (let i = 0; i < replyButtons.length; i++) {
		replyButtons[i].addEventListener("click", SetReplyToId);
	}

	const deleteCommentButtons = document.querySelectorAll(
		".js-delete-comment"
	);
	for (let i = 0; i < deleteCommentButtons.length; i++) {
		deleteCommentButtons[i].addEventListener("click", DeleteComment);
	}

	InitialiseLikeButtons();
};

export default InitialisePost;
