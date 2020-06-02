import InitialiseInputs from "./inputs";
import InitialiseAccountEdit from "./account-edit";
import InitialiseDropdowns from "./dropdown";
import InitialisePostCreate from "./post-create";
import InitialisePost from "./post";
import InitialiseProfile from "./profile";

const Load = () => {
	InitialiseInputs();
	InitialiseAccountEdit();
	InitialiseDropdowns();
	InitialisePostCreate();
	InitialisePost();
	InitialiseProfile();
};

if (document.readyState !== "loading") {
	Load();
} else {
	document.addEventListener("DOMContentLoaded", Load);
}
