#include "HelloWorldScene.h"
#include <SimpleAudioEngine.h>
#define MUSIC_FILE "666.mp3"
USING_NS_CC;

int count = 0;

Scene* HelloWorld::createScene()
{
    // 'scene' is an autorelease object
    auto scene = Scene::create();
    
    // 'layer' is an autorelease object
    auto layer = HelloWorld::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool HelloWorld::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !Layer::init() )
    {
        return false;
    }
    
    Size visibleSize = Director::getInstance()->getVisibleSize();
    Vec2 origin = Director::getInstance()->getVisibleOrigin();

    /////////////////////////////
    // 2. add a menu item with "X" image, which is clicked to quit the program
    //    you may modify it.

    // add a "close" icon to exit the progress. it's an autorelease object
    auto closeItem = MenuItemImage::create(
                                           "CloseNormal.png",
                                           "CloseSelected.png",
                                           CC_CALLBACK_1(HelloWorld::menuCloseCallback, this));
    
	closeItem->setPosition(Vec2(origin.x + visibleSize.width - closeItem->getContentSize().width/2 ,
                                origin.y + closeItem->getContentSize().height/2));
	auto dog = MenuItemImage::create(
		"timg.jpg",
		"1.jpg",
		CC_CALLBACK_1(HelloWorld::display_music, this)
	);
	dog->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
    // create menu, it's an autorelease object
    auto menu = Menu::create(closeItem, NULL);
    menu->setPosition(Vec2::ZERO);
    this->addChild(menu, 1);

	auto menu1 = Menu::create(dog, NULL);
	menu1->setPosition(Vec2::ZERO);
	this->addChild(menu1,1);

    /////////////////////////////
    // 3. add your codes below...

    // add a label shows "Hello World"
    // create and initialize a label
    
    //auto label = Label::createWithTTF("15331204", "fonts/Marker Felt.ttf", 24);
	CCDictionary* message = CCDictionary::createWithContentsOfFile("Chinese.xml");
	auto nameKey = message->valueForKey("Name");
	const char* Name = nameKey->getCString();
	auto IDKey = message->valueForKey("ID");
	const char* ID = IDKey->getCString();
	auto label = Label::createWithTTF(Name, "fonts/FZSTK.ttf", 24);
	auto label1 = Label::createWithTTF(ID, "fonts/Marker Felt.ttf", 24);
    // position the label on the center of the screen
    label->setPosition(Vec2(origin.x + visibleSize.width/2,
                            origin.y + visibleSize.height - label->getContentSize().height));
	label1->setPosition(Vec2(origin.x + visibleSize.width / 2,
		                    origin.y + visibleSize.height
		                     - label->getContentSize().height-label1->getContentSize().height));
    // add the label as a child to this layer
    this->addChild(label, 1);
	this->addChild(label1, 1);

    // add "HelloWorld" splash screen"
    auto sprite = Sprite::create("2.jpg");

    // position the sprite on the center of the screen
    sprite->setPosition(Vec2(visibleSize.width/2 + origin.x, visibleSize.height/2 + origin.y));

    // add the sprite as a child to this layer
    this->addChild(sprite, 0);
    
    return true;
}


void HelloWorld::menuCloseCallback(Ref* pSender)
{
    Director::getInstance()->end();

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    exit(0);
#endif
}

void HelloWorld::display_music(Ref* pSender) {
	CocosDenshion::SimpleAudioEngine::sharedEngine()->preloadBackgroundMusic(MUSIC_FILE);
	CocosDenshion::SimpleAudioEngine::sharedEngine()->setBackgroundMusicVolume(0.5);
	if (count % 2 == 0)
	  CocosDenshion::SimpleAudioEngine::sharedEngine()->playBackgroundMusic(MUSIC_FILE, true);
	else
	  CocosDenshion::SimpleAudioEngine::sharedEngine()->pauseBackgroundMusic();
	count++;
}