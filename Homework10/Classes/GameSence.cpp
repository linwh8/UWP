#include "GameSence.h"
# include "StoneLayer.h"
# include "MouseLayer.h"

USING_NS_CC;

Scene* GameSence::createScene()
{
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = GameSence::create();

	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}

// on "init" you need to initialize your instance
bool GameSence::init()
{

	if (!Layer::init())
	{
		return false;
	}

	//add touch listener
	EventListenerTouchOneByOne* listener = EventListenerTouchOneByOne::create();
	listener->setSwallowTouches(true);
	listener->onTouchBegan = CC_CALLBACK_2(GameSence::onTouchBegan, this);
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(listener, this);


	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	auto bg = Sprite::create("level-background-0.jpg");
	bg->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
	this->addChild(bg, 0);

	/*StoneLayer* stone = StoneLayer::create();
	MouseLayer* mouse = MouseLayer::create();
	stone->setPosition(Vec2(0,0));
	stone->setAnchorPoint(Vec2(0,0));
	stone->ignoreAnchorPointForPosition(false);
	mouse->setPosition(Vec2(0,origin.y+visibleSize.height/2));
	mouse->setAnchorPoint(Vec2(0, 0));
	mouse->ignoreAnchorPointForPosition(false);
	this->addChild(stone, 1);
	this->addChild(mouse, 1);
	*/
	auto spritecache = SpriteFrameCache::getInstance();
	spritecache->addSpriteFramesWithFile("level-sheet.plist");
	mouse = Sprite::createWithSpriteFrameName("gem-mouse-0.png");
	mouse->setPosition(Vec2(origin.x + visibleSize.width / 2, 0));
	Animate* mouseAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("mouseAnimation"));
	mouse->runAction(RepeatForever::create(mouseAnimate));

	stone = Sprite::create("stone.png");
	stone->setPosition(Vec2(560, 480));

	stoneLayer = Layer::create();
	stoneLayer->setPosition(Vec2(0, 0));
	stoneLayer->setAnchorPoint(Vec2(0, 0));
	stoneLayer->ignoreAnchorPointForPosition(false);
	stoneLayer->addChild(stone);


	mouseLayer = Layer::create();
	mouseLayer->setPosition(Vec2(0, origin.y + visibleSize.height / 2));
	mouseLayer->setAnchorPoint(Vec2(0, 0));
	mouseLayer->ignoreAnchorPointForPosition(false);
	mouseLayer->addChild(mouse);

	this->addChild(stoneLayer, 1);
	this->addChild(mouseLayer, 1);


	auto shoot = Label::createWithSystemFont("Shoot", "Marker Felt", 80);
	auto menuItem = MenuItemLabel::create(shoot,CC_CALLBACK_1(GameSence::menuCallback, this));
	menuItem->setPosition(Vec2(visibleSize.width+origin.x-150, visibleSize.height+origin.y-140));

	auto menu = Menu::create(menuItem, NULL);
	menu->setAnchorPoint(Vec2::ZERO);
	menu->setPosition(Vec2::ZERO);
	this->addChild(menu, 1);
/*

	auto stone = Sprite::create("stone.png");
	stone->setPosition(Vec2(560,480));
	this->addChild(stone, 1);*/


	return true;
}

bool GameSence::onTouchBegan(Touch *touch, Event *unused_event) {

	auto location = touch->getLocation();
	auto cheese = Sprite::create("cheese.png");
	cheese->setPosition(location);
	this->addChild(cheese);
	auto moveTo = MoveTo::create(2, mouseLayer->convertToNodeSpace(location));
	mouse->runAction(moveTo);
	//this->removeChild(cheese);
	auto delayTime = DelayTime::create(2);
	auto func = CallFunc::create([this, cheese]() {
		this->removeChild(cheese);
	});
	auto seq = Sequence::create(delayTime, func, NULL);
	this->runAction(seq);
	return true;
}

void GameSence::menuCallback(cocos2d::Ref* pSender) {
	auto origin = Point(30, 30);
	if (mouse->getPosition() != mouseLayer->convertToNodeSpace(origin)) {
		auto location = mouse->getPosition();
		location = mouseLayer->convertToWorldSpace(location);
		auto moveTo = MoveTo::create(1, stoneLayer->convertToNodeSpace(location));
		stone->runAction(moveTo);
		auto delayTime = DelayTime::create(0.5);
		auto func = CallFunc::create([this, location]() {
			auto _location = Point(30, 30);
			auto moveTo = MoveTo::create(1, mouseLayer->convertToNodeSpace(_location));
			mouse->runAction(moveTo);
			/*
			Animate* legAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("legAnimation"));
	        leg->runAction(RepeatForever::create(legAnimate));
			*/
			auto diamond = Sprite::create("diamond.png");
			Animate* diamondAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("diamondAnimation"));
			diamond->runAction(RepeatForever::create(diamondAnimate));
			diamond->setPosition(this->convertToNodeSpace(location));
			this->addChild(diamond);
			auto _delayTime = DelayTime::create(0.5);
			auto _func = CallFunc::create([this]() {
				stoneLayer->removeChild(stone);
				stone = Sprite::create("stone.png");
				stone->setPosition(Vec2(560, 480));
				stoneLayer->addChild(stone);
			});
			auto _seq = Sequence::create(_delayTime, _func, NULL);
			this->runAction(_seq);
		});
		auto seq = Sequence::create(delayTime, func, NULL);
		this->runAction(seq);
	}
}
